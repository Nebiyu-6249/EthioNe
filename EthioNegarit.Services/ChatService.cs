namespace EthioNegarit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Core.Constants;
    using Core.Domain.Entities;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    using Utilities;
    using System.Web;
    using Core.Exceptions;
    using Core.Domain.General;
    public partial class ChatService : IChatService
    {
        private readonly EthioNegaritContext _context;
        private readonly IBadgeService _badgeService;
        private readonly ILoggingService _loggingService;
        private readonly ICacheService _cacheService;
        private readonly IMembershipUserPointsService _membershipUserPointsService;
        private readonly ISettingsService _settingsService;
        public ChatService(IBadgeService badgeService, ILoggingService loggingService, IEthioNegaritContext context, ICacheService cacheService, IMembershipUserPointsService membershipUserPointsService, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _badgeService = badgeService;
            _loggingService = loggingService;
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;
            _membershipUserPointsService = membershipUserPointsService;
        }      
        public ChatRoom SanitizeChatRoom(ChatRoom chatRoom)
        {
            // Sanitize any strings in a ChatRoom
            chatRoom.Description = StringUtils.GetSafeHtml(chatRoom.Description);
            chatRoom.Name = HttpUtility.HtmlDecode(StringUtils.SafePlainText(chatRoom.Name));
            return chatRoom;
        }
        public ChatRoom GetBySlug(string slug)
        {
            slug = StringUtils.GetSafeHtml(slug);
            var cacheKey = string.Concat(CacheKeys.ChatRoom.StartsWith, "GetBySlug-", slug);
            return _cacheService.CachePerRequest(cacheKey, () => _context.ChatRoom.FirstOrDefault(x => x.Slug == slug));
        }
        public ChatRoom GetChatRoom(string slug)
        {
            slug = StringUtils.GetSafeHtml(slug);
            var cacheKey = string.Concat(CacheKeys.ChatRoom.StartsWith, "GetBySlug-", slug);
            return _cacheService.CachePerRequest(cacheKey, () => _context.ChatRoom.FirstOrDefault(x => x.Slug == slug));
        }
        //Get Chat Room
        public ChatRoom Get(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.ChatRoom.StartsWith, "Get-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.ChatRoom.FirstOrDefault(x => x.Id == id));
        }
        public IList<ChatRoom> GetBySlugLike(string slug)
        {
            slug = StringUtils.GetSafeHtml(slug);
            var cacheKey = string.Concat(CacheKeys.ChatRoom.StartsWith, "GetBySlugLike-", slug);
            return _cacheService.CachePerRequest(cacheKey, () => _context.ChatRoom.Where(x => x.Slug.Contains(slug))                                                                    .ToList());
        }
        public IList<ChatRoom> GetChatRooms()
        {
            var cacheKey = string.Concat(CacheKeys.ChatRoom.StartsWith, "GetChatRooms");
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.ChatRoom
                        .Include(x => x.ChatMessage)           
                         .AsNoTracking()
                        .OrderBy(x => x.Name)
                        .ToList();
            });
        }
        public ChatRoom Add(ChatRoom chatRoom)
        {
            // Sanitize
            chatRoom = SanitizeChatRoom(chatRoom);
            // Set the create date
            chatRoom.DateCreated = DateTime.UtcNow;
            // url slug generator
            chatRoom.Slug = ServiceHelpers.GenerateSlug(chatRoom.Name, GetBySlugLike(ServiceHelpers.CreateUrl(chatRoom.Name)), null);
            // Add the ChatRoom
            return _context.ChatRoom.Add(chatRoom);
        }
        public void UpdateSlugFromName(ChatRoom chatRoom)
        {
            // Sanitize
            chatRoom = SanitizeChatRoom(chatRoom);
            var updateSlug = true;
            // Check if slug has changed as this could be an update
            if (!string.IsNullOrEmpty(chatRoom.Slug))
            {
                var chatRoomBySlug = GetBySlug(chatRoom.Slug);
                if (chatRoomBySlug.Id == chatRoom.Id)
                {
                    updateSlug = false;
                }
            }
            if (updateSlug)
            {
                chatRoom.Slug = ServiceHelpers.GenerateSlug(chatRoom.Name, GetBySlugLike(chatRoom.Slug), chatRoom.Slug);
            }
        }
        public void Delete(ChatRoom chatRoom)
        {
            // Check if anyone else if using this role
            var okToDelete = !chatRoom.ChatMessage.Any();
            if (okToDelete)
            {
                _context.ChatRoom.Remove(chatRoom);
            }
            else
            {
                var inUseBy = new List<Entity>();
                inUseBy.AddRange(chatRoom.ChatMessage);
                throw new InUseUnableToDeleteException(inUseBy);
            }
        }        
        public ChatRoom GetChatRoom(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.ChatRoom.StartsWith, "Get-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.ChatRoom.FirstOrDefault(x => x.Id == id));
        }   
        public ChatMessage AddMessage(ChatMessage chatMessage)
        {
            // Sanitize
            chatMessage = SanitizeChatMessage(chatMessage);   
            return _context.ChatMessage.Add(chatMessage);
        }
        public ChatMessage SanitizeChatMessage(ChatMessage chatMessage)
        {
            // Sanitize any strings in a message Content
            chatMessage.Content = StringUtils.GetSafeHtml(chatMessage.Content);
            return chatMessage;
        }
        public void Delete(ChatMessage message)
        {
            // Delete any points associated with this message
            // _membershipUserPointsService.Delete(PointsFor.Like, message.Id);
            // Delete the Response
            _context.ChatMessage.Remove(message);
        }
        public IList<ChatRoomMember> GetRoomSubscription(MembershipUser user, ChatRoom chatRoom, bool addTracking = false)
        {
            var subscriptions = _context.ChatRoomMember
                .Where(x => x.User.Id == user.Id && x.ChatRoom.Id == chatRoom.Id);
            if (addTracking)
            {
                return subscriptions.ToList();
            }
            return subscriptions.AsNoTracking().ToList();
        }      
        public ChatRoomMember AddMember(ChatRoomMember chatRoomMember)
        {
            return _context.ChatRoomMember.Add(chatRoomMember);         
        }
        public void DeleteSubscription(ChatRoomMember chatRoomMember)
        {
            _context.ChatRoomMember.Remove(chatRoomMember);
        }
        public ChatRoomMember GetRoomSubscription(Guid userId, Guid chatRoomId)
        {
            var subscription = _context.ChatRoomMember
               .FirstOrDefault(x => x.User.Id == userId && x.ChatRoom.Id == chatRoomId);       
            return subscription;
        }
		public IList<ChatResponse> GetResponses(List<Guid> messageIds)
        {
            var cacheKey = string.Concat(CacheKeys.ChatResponse.StartsWith, "GetResponses-", messageIds.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {            
                return _context.ChatResponse
                    .Include(x => x.ChatMessage)
                    .Include(x => x.User)
                    .AsNoTracking()
                    .Where(x => messageIds.Contains(x.ChatMessage.Id))
                    .ToList();
            });
        }
        public IList<ChatMessage> GetReplies(Guid messageId)
        {
            var replies = _context.ChatMessage
                // .Where(x => x.InReplyTo == messageId && x.Deleted == false);
                .Where(x => x.InReplyTo == messageId)
                .ToList();
            return replies;
        }
        public void DeleteResponse(ChatResponse chatResponse)
        {
            // Delete any points associated with this Response
            //  _membershipUserPointsService.Delete(PointsFor.Like, like.Id);
            // Delete Response
            _context.ChatResponse.Remove(chatResponse);
        }
        public ChatResponse AddResponse(ChatResponse chatResponse)
                {
                        _context.ChatResponse.Add(chatResponse);
                    return chatResponse;
                }
        public PagedList<ChatRoomMember> GetRoomMembers(Guid chatRoomId,int pageIndex, int pageSize, int amountToTake)
        {
            var total = _context.ChatRoomMember.AsNoTracking().Count(x => x.ChatRoom.Id == chatRoomId);
             if (amountToTake < total)
             {
                 total = amountToTake;
             }
            var results = _context.ChatRoomMember
                               .Include(x => x.User)
                               .Include(x => x.ChatRoom)
                                //.OrderBy(x => x.LastActive)
                                .OrderByDescending(x => x.LastActive)                        
                               // .ThenBy(x => x.LogOn)
                                .Where(x => x.ChatRoom.Id == chatRoomId)
                                //.Skip((pageIndex - 1) * pageSize)
                              //  .Take(pageSize)
                                 .Skip((pageIndex - 1) * amountToTake)
                                .Take(amountToTake)
                                .ToList();
            return new PagedList<ChatRoomMember>(results, pageIndex, amountToTake, total);
        }
        public PagedList<ChatRoom> GetRooms(int pageIndex, int pageSize, int amountToTake,bool freezed = false)
        {
            var total = _context.ChatRoom.AsNoTracking().Count(x => x.Freeze == freezed);
            if (amountToTake < total)
            {
                total = amountToTake;
            }
            var results = _context.ChatRoom
                .AsNoTracking()
                                 .Where(x => x.Freeze == freezed)
                                 .OrderBy(x => x.LastActive)
                                 .Skip((pageIndex - 1) * amountToTake)
                                .Take(amountToTake)
                                .ToList();
            return new PagedList<ChatRoom>(results, pageIndex, amountToTake, total);
        }
        public ChatMessage GetMessage(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.ChatMessage.StartsWith, "GetMessage-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.ChatMessage.FirstOrDefault(x => x.Id == id));
        }      
        public IList<ChatResponse> GetReports(Guid userId, DateTime? from)
        {
            if (from == null)
            {
                from = DateTime.UtcNow.AddDays(-14);
            }
           
            return _context.ChatResponse
                    .Include(x => x.ChatMessage)
                    .Include(x => x.User)
                    .AsNoTracking()
                    .Where(x => x.Amount < 0 && x.User.Id == userId && x.DateResponse > from)
                    .ToList();          
        }
        public PagedList<ChatMessage> GetRoomMessages(int pageIndex, int pageSize, int amountToTake, Guid chatRoomId)
        {
            var cacheKey = string.Concat(CacheKeys.ChatMessage.StartsWith, "GetRoomMessages-", pageIndex, "-", pageSize, "-", amountToTake, "-", chatRoomId);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var total = _context.ChatMessage.AsNoTracking().Count(x => x.ChatRoom.Id == chatRoomId && x.FlaggedAsSpam != true);
                if (amountToTake < total)
                {
                    total = amountToTake;
                }
                // Get the messages using an efficient
                var results = _context.ChatMessage
                                      .Include(x => x.User)
                                      .AsNoTracking()
                                         .Where(x => x.ChatRoom.Id == chatRoomId && x.FlaggedAsSpam == false && x.IsReplay == false && x.InReplyTo == null)
                .OrderByDescending(x => x.Timestamp)
               .ToList();
                var chatMessages = results.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return new PagedList<ChatMessage>(chatMessages, pageIndex, pageSize, total);
            });
        }
        public int FreezedRoomsCount()
        {
                return _context.ChatRoom
                    .AsNoTracking()
                    .Count(x => x.Freeze == true);
        }
    }
}
