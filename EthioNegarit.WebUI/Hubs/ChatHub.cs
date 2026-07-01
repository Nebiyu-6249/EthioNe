using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using EthioNegarit.Services.Data.Context;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.WebUI.ViewModels;
using EthioNegarit.WebUI.Application;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.WebUI.ViewModels.Mapping;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.Utilities;

namespace EthioNegarit.WebUI.Hubs
{
   [Authorize]
    public class ChatHub : Hub
    {
        //  private readonly EthioNegaritContext _db = new EthioNegaritContext();
        private readonly EthioNegaritContext _context;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;
        protected readonly IMembershipService MembershipService;
        protected MembershipUser LoggedOnReadOnlyUser;
        private readonly IChatService _chatService;
     
        public ChatHub(EthioNegaritContext dataContext ,IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, IChatService chatService)
        {
            _context = dataContext;
            UnitOfWorkManager = unitOfWorkManager;
            MembershipService = membershipService;
           // _chatService = ServiceFactory.Get<IChatService>();
           _chatService = chatService;
           LoggedOnReadOnlyUser = UserIsAuthenticated ? ServiceFactory.Get<IMembershipService>().GetUser(Username, true) : null;
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                LoggedOnReadOnlyUser = UserIsAuthenticated ? MembershipService.GetUser(Username, true) : null;
           
            }
        }
        protected bool UserIsAuthenticated => System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

        protected string Username => UserIsAuthenticated ? System.Web.HttpContext.Current.User.Identity.Name : null;

        // Client calls this after the connection starts to subscribe to a room.
        public void JoinRoom(Guid roomId)
        {
            // leave any current room groups first
            /*    var oldRoomId = GetCurrentRoomId();
                if (oldRoomId > 0 && oldRoomId != roomId)
                    Groups.Remove(Context.ConnectionId, RoomGroup(oldRoomId))

                         Groups.Add(Context.ConnectionId, RoomGroup(roomId));
                _repo.AddConnection(Context.ConnectionId, userId, roomId);

                var users = _repo.GetOnlineUsers(roomId);
                Clients.Caller.roomUsers(users);*/

//U.AddConnection(Context.ConnectionId, userId, roomId);
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                Groups.Add(Context.ConnectionId, GroupName(roomId));
                //  Clients.Group(GroupName(roomId)).refreshOnlineUsers();
                Clients.All.refreshOnlineUsers();
            }
        }

        // Client calls this when leaving / navigating away.
        public void LeaveRoom(Guid roomId)
        {
            Groups.Remove(Context.ConnectionId,GroupName(roomId));
        }

        /// <summary>
        /// Persists the message to the database, then broadcasts it to everyone
        /// in the room (including the sender, so the UI stays consistent).
        /// </summary>
        /// //
        public void sendMessage(Guid roomId, string content)
        {
          // content = Sanitize(content);
           // if (string.IsNullOrEmpty(content)) return;
          //  if (content.Length > 2000) content = content.Substring(0, 2000);
       // SaveAndBroadcast(roomId, content, parentId: null);
            Clients.Group(GroupName(roomId)).updateMessages();
        }
        // ── Private helpers ──────────────────────────────────────────────────
        private void SaveAndBroadcast(Guid roomId, string content, Guid? parentId)
        {
            var SettingsService = ServiceFactory.Get<ISettingsService>();
            var settings = SettingsService.GetSettings();
            // var settings = SettingsService.GetSettings();
            // var chatService = ServiceFactory.Get<IChatService>();
            //  var room = ServiceFactory.Get<IChatService>().Get(roomId);

            //    LoggedOnReadOnlyUser = UserIsAuthenticated ? ServiceFactory.Get<IMembershipService>().GetUser(Username, true) : null;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var room = _chatService.Get(roomId);
                var msg = new ChatMessage
                {
                    ChatRoom = room,
                    User = LoggedOnReadOnlyUser,
                    Content = content,
                    Timestamp = DateTime.UtcNow,
                    FlaggedAsSpam = false,
                    Deleted = false,
                    // InReplyTo = model.InReplyTo,
                    // IsReplay = model.IsReplay
                    InReplyTo = null,
                    IsReplay = false
                };
                msg = _chatService.AddMessage(msg);
                try
                {
                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                }
                var viewModel = ViewModelMapping.CreateMessageViewModel(msg, new List<ChatResponse>(), LoggedOnReadOnlyUser, settings);
               // Clients.Group(GroupName(roomId)).receiveMessage(viewModel);
                Clients.Group(GroupName(roomId)).updateMessages();
            }
            
        }
     
       private static string GroupName(Guid roomId)
        {
            string room = roomId.ToString();
            return "room-" + room;
        }
        /* private Guid GetCurrentRoomId()
         {
             // passed via query string: /chatHub?roomId=1
             var qs = Context.QueryString["roomId"];
             if (Guid.TryParse(qs, out Guid id)) {
             return id;
         }
             return Guid.Empty;
         }*/
        /*private static string Sanitize(string s)
        {
            if (s == null) return "";
            return System.Web.HttpUtility.HtmlEncode(s.Trim());
        }*/
        /*
          public override Task OnConnected()
        {
            var userId = GetCurrentUserId();
            var roomId = GetCurrentRoomId();

            if (userId > 0 && roomId > 0)
            {
                Groups.Add(Context.ConnectionId, RoomGroup(roomId));
                _repo.AddConnection(Context.ConnectionId, userId, roomId);

                var user = _repo.GetUser(userId);
                Clients.Group(RoomGroup(roomId)).userJoined(new UserViewModel
                {
                    UserId    = user.UserId,
                    Username  = user.Username,
                    AvatarUrl = user.GetAvatar(),
                    Reputation = user.Reputation,
                    IsOnline  = true
                });

                // send a system message
                Clients.Group(RoomGroup(roomId)).systemMessage(
                    $"{user.Username} has entered the room.");
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var userId = _repo.RemoveConnection(Context.ConnectionId);

            if (userId.HasValue)
            {
                var user = _repo.GetUser(userId.Value);
                if (user != null)
                {
                    // only broadcast "left" if user has no other active connections
                    Clients.All.userLeft(userId.Value, user.Username);
                    Clients.All.systemMessage($"{user.Username} has left.");
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var userId = GetCurrentUserId();
            var roomId = GetCurrentRoomId();
            if (userId > 0 && roomId > 0)
            {
                Groups.Add(Context.ConnectionId, RoomGroup(roomId));
                _repo.AddConnection(Context.ConnectionId, userId, roomId);
            }
            return base.OnReconnected();
        }

        */
    }
}
/*  
using (var db = new EthioNegaritContext())
  {
      db.ChatMessage.Add(msg);
      db.SaveChanges();
  }
  */
// Reload with navigation props
/*  msg = await db.ChatMessage
      .Include(m => m.User)
      .Include(m => m.ChatResponses)
      .Include(m => m.ChatRoom)
      .FirstAsync(m => m.Id == msg.Id)*/
