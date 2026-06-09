using System;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using EthioNegarit.Services.Data.Context;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.WebUI.ViewModels;

namespace EthioNegarit.WebUI.Hubs
{
    public class ChatHub : Hub
    {
        // connectionId -> roomId(string). In-memory presence; resets on app restart.
        private static readonly ConcurrentDictionary<string, string> ConnectionRoom
            = new ConcurrentDictionary<string, string>();

        // The authenticated username (MVCForum's Identity.Name), or null.
        private string CurrentUsername =>
            Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated
                ? Context.User.Identity.Name
                : null;

        // ── Connection lifecycle ──────────────────────────────────────────────
        public override Task OnDisconnected(bool stopCalled)
        {
            string roomId;
            if (ConnectionRoom.TryRemove(Context.ConnectionId, out roomId) && roomId != null)
            {
                Clients.Group(roomId).userLeft(GetDisplayName(), OnlineCount(roomId));
            }
            return base.OnDisconnected(stopCalled);
        }

        // ── Hub methods (called from the browser) ─────────────────────────────

        /// <summary>Join a room. Called once on page load. Sends back history + presence.</summary>
        public async Task JoinRoom(Guid roomId)
        {
            using (var db = new EthioNegaritContext())
            {
                var room = await db.ChatRoom.FindAsync(roomId);
                if (room == null) { Clients.Caller.error("Room not found."); return; }

                var group = roomId.ToString();

                // Leave a previous room if this connection was in one.
                string oldGroup;
                if (ConnectionRoom.TryGetValue(Context.ConnectionId, out oldGroup) && oldGroup != group)
                {
                    await Groups.Remove(Context.ConnectionId, oldGroup);
                    Clients.Group(oldGroup).userLeft(GetDisplayName(), OnlineCount(oldGroup));
                }

                ConnectionRoom[Context.ConnectionId] = group;
                await Groups.Add(Context.ConnectionId, group);

                var me = GetCurrentUser(db);
                var myId = me?.Id ?? Guid.Empty;

                // Last 50 non-deleted messages, oldest-first for display.
                var recent = await db.ChatMessage
                    .Include(m => m.User)
                    .Include(m => m.ChatResponses)
                    .Where(m => m.ChatRoom.Id == roomId && m.Deleted != true)
                    .OrderByDescending(m => m.Timestamp)
                    .Take(50)
                    .ToListAsync();
                recent.Reverse();

                var history = recent.Select(m => ToViewModel(db, m, myId)).ToList();

                Clients.Caller.loadHistory(history);
                Clients.Caller.roomJoined(room.Id, room.Name, OnlineCount(group));
                Clients.OthersInGroup(group).userJoined(GetDisplayName(), OnlineCount(group));
            }
        }

        /// <summary>Send a new top-level message.</summary>
        public async Task SendMessage(Guid roomId, string body)
        {
            body = Clean(body);
            if (string.IsNullOrEmpty(body)) return;
            if (body.Length > 2000) body = body.Substring(0, 2000);
            await SaveAndBroadcast(roomId, body, null);
        }

        /// <summary>Reply to an existing message (threading).</summary>
        public async Task ReplyToMessage(Guid roomId, Guid parentMessageId, string body)
        {
            body = Clean(body);
            if (string.IsNullOrEmpty(body)) return;
            if (body.Length > 2000) body = body.Substring(0, 2000);
            await SaveAndBroadcast(roomId, body, parentMessageId);
        }

        /// <summary>Toggle a star/reaction on a message.</summary>
        public async Task ToggleStar(Guid messageId)
        {
            using (var db = new EthioNegaritContext())
            {
                var me = GetCurrentUser(db);
                if (me == null) { Clients.Caller.error("You must be signed in."); return; }

                var msg = await db.ChatMessage.FindAsync(messageId);
                if (msg == null) return;

                var existing = await db.ChatResponse
                    .FirstOrDefaultAsync(r => r.ChatMessage.Id == messageId && r.User.Id == me.Id);

                bool starred;
                if (existing != null)
                {
                    db.ChatResponse.Remove(existing);
                    starred = false;
                }
                else
                {
                    db.ChatResponse.Add(new ChatResponse
                    {
                        ChatMessage = msg,
                        User = me,
                        MembershipUserRespond = me,
                        Amount = 1,
                        DateResponse = DateTime.UtcNow
                    });
                    starred = true;
                }
                await db.SaveChangesAsync();

                var count = await db.ChatResponse.CountAsync(r => r.ChatMessage.Id == messageId);

                string group;
                if (ConnectionRoom.TryGetValue(Context.ConnectionId, out group) && group != null)
                    Clients.Group(group).messageStarred(messageId, count, starred);
            }
        }

        /// <summary>Soft-delete your own message.</summary>
        public async Task DeleteMessage(Guid messageId)
        {
            using (var db = new EthioNegaritContext())
            {
                var me = GetCurrentUser(db);
                if (me == null) { Clients.Caller.error("You must be signed in."); return; }

                var msg = await db.ChatMessage.Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.Id == messageId);

                if (msg == null || msg.User == null || msg.User.Id != me.Id)
                {
                    Clients.Caller.error("You can only delete your own messages.");
                    return;
                }

                msg.Deleted = true;
                await db.SaveChangesAsync();

                string group;
                if (ConnectionRoom.TryGetValue(Context.ConnectionId, out group) && group != null)
                    Clients.Group(group).messageDeleted(messageId);
            }
        }

        /// <summary>Create a new room and tell everyone.</summary>
        public async Task CreateRoom(string name, string description)
        {
            name = Clean(name);
            if (string.IsNullOrEmpty(name)) return;

            using (var db = new EthioNegaritContext())
            {
                if (await db.ChatRoom.AnyAsync(r => r.Name == name))
                {
                    Clients.Caller.error("A room with that name already exists.");
                    return;
                }

                var room = new ChatRoom
                {
                    Name = name,
                    Description = Clean(description ?? string.Empty),
                    Slug = Slugify(name),
                    DateCreated = DateTime.UtcNow,
                    LastActive = DateTime.UtcNow
                };
                db.ChatRoom.Add(room);
                await db.SaveChangesAsync();

                Clients.All.newRoomCreated(new { id = room.Id, name = room.Name, description = room.Description, slug = room.Slug });
            }
        }

        /// <summary>Return the full room list to the caller.</summary>
        public async Task GetRooms()
        {
            using (var db = new EthioNegaritContext())
            {
                var rooms = await db.ChatRoom
                    .OrderBy(r => r.Name)
                    .Select(r => new { id = r.Id, name = r.Name, description = r.Description, slug = r.Slug })
                    .ToListAsync();

                Clients.Caller.roomsList(rooms);
            }
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private async Task SaveAndBroadcast(Guid roomId, string body, Guid? parentId)
        {
            using (var db = new EthioNegaritContext())
            {
                var author = GetCurrentUser(db);
                if (author == null) { Clients.Caller.error("You must be signed in to send messages."); return; }

                var room = await db.ChatRoom.FindAsync(roomId);
                if (room == null) { Clients.Caller.error("Room not found."); return; }

                var msg = new ChatMessage
                {
                    Content = body,
                    ChatRoom = room,
                    User = author,
                    Timestamp = DateTime.UtcNow,
                    InReplyTo = parentId,
                    IsReplay = parentId.HasValue,
                    Deleted = false,
                    FlaggedAsSpam = false,
                    DisableReplay = false
                };

                db.ChatMessage.Add(msg);
                room.LastActive = DateTime.UtcNow;
                await db.SaveChangesAsync();

                var vm = ToViewModel(db, msg, author.Id);

                // Broadcast to everyone in the room — INCLUDING the sender, so the
                // sender sees their own message land.
                Clients.Group(roomId.ToString()).receiveMessage(vm);
            }
        }

        private MessageViewModel ToViewModel(EthioNegaritContext db, ChatMessage m, Guid currentUserId)
        {
            string parentPreview = null, parentAuthor = null;
            if (m.InReplyTo.HasValue)
            {
                var parent = db.ChatMessage.Include(p => p.User)
                    .FirstOrDefault(p => p.Id == m.InReplyTo.Value);
                if (parent != null)
                {
                    var c = parent.Content ?? string.Empty;
                    parentPreview = c.Length > 60 ? c.Substring(0, 60) + "…" : c;
                    parentAuthor = parent.User?.UserName;
                }
            }

            var responses = m.ChatResponses;
            return new MessageViewModel
            {
                Id = m.Id,
                Content = m.Content,
                AuthorName = m.User?.UserName ?? "Unknown",
                AuthorId = m.User?.Id ?? Guid.Empty,
                GravatarHash = GravatarHash(m.User?.Email),
                SentAt = m.Timestamp.ToString("o"),
                StarCount = responses?.Count ?? 0,
                IsStarred = responses != null && responses.Any(r => r.User != null && r.User.Id == currentUserId),
                ParentId = m.InReplyTo,
                ParentPreview = parentPreview,
                ParentAuthor = parentAuthor
            };
        }

        private MembershipUser GetCurrentUser(EthioNegaritContext db)
        {
            var name = CurrentUsername;
            if (string.IsNullOrEmpty(name)) return null;
            return db.MembershipUser.FirstOrDefault(u => u.UserName == name);
        }

        private string GetDisplayName()
        {
            return CurrentUsername ?? "Someone";
        }

        private int OnlineCount(string group)
        {
            return ConnectionRoom.Values.Count(r => r == group);
        }

        private static string GravatarHash(string email)
        {
            if (string.IsNullOrEmpty(email)) return "00000000000000000000000000000000";
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant()));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }

        // Store raw, trimmed text. The view escapes on render, so the browser is the
        // single source of truth for XSS safety (avoids double-encoding).
        private static string Clean(string s)
        {
            return s?.Trim() ?? string.Empty;
        }

        private static string Slugify(string s)
        {
            if (string.IsNullOrEmpty(s)) return Guid.NewGuid().ToString("n");
            var slug = new string(s.Trim().ToLowerInvariant()
                .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-').ToArray());
            while (slug.Contains("--")) slug = slug.Replace("--", "-");
            return slug.Trim('-');
        }
    }
}
