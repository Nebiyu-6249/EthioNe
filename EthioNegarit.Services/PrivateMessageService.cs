namespace EthioNegarit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Core.Constants;
    using Core.Domain.Entities;
    using Core.Domain.General;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    using Utilities;
    using Core.Events;
    using System.Web.Hosting;

    public partial class PrivateMessageService : IPrivateMessageService
    {      
         private readonly EthioNegaritContext _context;
        private readonly ICacheService _cacheService;
        private readonly IAttachmentService _attachmentService;
        public PrivateMessageService(IEthioNegaritContext context, ICacheService cacheService, IAttachmentService attachmentService)
        {
            _cacheService = cacheService;
            _attachmentService = attachmentService;
            _context = context as EthioNegaritContext;
        }
        public PrivateMessage SanitizeMessage(PrivateMessage privateMessage)
        {
            privateMessage.Message = StringUtils.GetSafeHtml(privateMessage.Message);
            return privateMessage;
        }       
        public PrivateMessage Add(PrivateMessage message)
        {
            // This is the message that the other user sees
            message = SanitizeMessage(message);
            message.DateSent = DateTime.UtcNow;
            message.IsRead = false;
            message.IsStarred = false;
            message.IsSentMessage = false;

          message = _context.PrivateMessage.Add(message);
            var e = new PrivateMessageEventArgs { PrivateMessage = message };
            EventManager.Instance.FireBeforePrivateMessage(this, e);

            if (!e.Cancel)
            {
               // message = _context.PrivateMessage.Add(message);
                // We create a sent message that sits in the users sent folder, this is 
                // so that if the receiver deletes the message - The sender still has a record of it.
                var sentMessage = new PrivateMessage
                {
                    IsSentMessage = true,
                    DateSent = message.DateSent,
                    Message = message.Message,
                    UserFrom = message.UserFrom,
                    UserTo = message.UserTo,
                    IsRead = false,
                    IsStarred = false
            };

                _context.PrivateMessage.Add(sentMessage);

                EventManager.Instance.FireAfterPrivateMessage(this, new PrivateMessageEventArgs { PrivateMessage = message });
            }
            // Return the main message
            return message;
        }       
        public PrivateMessage Get(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.PrivateMessage.StartsWith, "Get-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.PrivateMessage
                                                                        .Include(x => x.UserTo)
                                                                        .Include(x => x.UserFrom)
                                                                        .Include(x => x.Attachments)
                                                                        .FirstOrDefault(x => x.Id == id));
        }
		public IPagedList<PrivateMessageListItem> GetUserPrivateMessages(int pageIndex, int pageSize, MembershipUser user)
        {
            var total = _context.PrivateMessage.Where(x => x.UserFrom.Id == user.Id || x.UserTo.Id == user.Id).ToList().DistinctBy(x => x.UserFrom);
            var query = _context.PrivateMessage
                .AsNoTracking()
                .Include(x => x.UserFrom)
                .Include(x => x.UserTo)
                .Include(x => x.Message)
                .Include(x => x.Attachments)
                 .Where(x => (x.UserTo.Id == user.Id && x.IsSentMessage != true) || (x.UserFrom.Id == user.Id && x.IsSentMessage == true))
                .Select(x => new PrivateMessageListItem
                {
                    IsSentMessage = (x.IsSentMessage == true && x.UserFrom.Id == user.Id) || (x.IsSentMessage == true && x.UserTo.Id != user.Id),
                    IsReceivedMessage = (x.IsSentMessage != true && x.UserFrom.Id != user.Id) || (x.IsSentMessage != true || x.UserTo.Id == user.Id),
                    HasUnreadMessages = ((x.IsSentMessage != true && x.UserFrom.Id != user.Id) && (x.IsRead == false)),
                    UnRead = (x.IsRead != true && x.UserTo.Id == user.Id),
                    User = (x.IsSentMessage == true ? x.UserTo : x.UserFrom),
                    IsStarred = x.IsStarred,
                    Message = x.Message,
                    MessageId = x.Id,
                    Date = x.DateSent,
                    Hasattachments = (x.Attachments.Any()),
                     AttachmentCount = (x.Attachments.Count)

                })
             .OrderByDescending(x => x.Date);
          
            var results = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            // Return a paged list
            return new PagedList<PrivateMessageListItem>(results, pageIndex, pageSize, total.Count());
        }
        public IPagedList<PrivateMessage> GetUsersConversation(int pageIndex, int pageSize, MembershipUser toUser, MembershipUser fromUser)
        {
            var query = _context.PrivateMessage
               .AsNoTracking()
               .Include(x => x.UserFrom)
               .Include(x => x.UserTo)
               .Include(x => x.Attachments)
              // .Where(x => (x.UserFrom.Id == fromUser.Id && x.UserTo.Id == toUser.Id && x.IsSentMessage != true) || (x.UserFrom.Id == toUser.Id && x.UserTo.Id == fromUser.Id && x.IsSentMessage == true))
                .Where(x => (x.UserFrom.Id == fromUser.Id && x.UserTo.Id == toUser.Id && x.IsSentMessage != true) || (x.UserFrom.Id == toUser.Id && x.UserTo.Id == fromUser.Id && x.IsSentMessage == true))
               .OrderByDescending(x => x.DateSent);
            var total = query.Count();
            var results = query
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
            // Return a paged list
            return new PagedList<PrivateMessage>(results, pageIndex, pageSize, total);
        }

        /// <summary>
        /// Gets the last sent private message from a specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PrivateMessage GetLastSentPrivateMessage(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.PrivateMessage.StartsWith, "GetLastSentPrivateMessage-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.PrivateMessage
                                                                       // .AsNoTracking()
                                                                      // .Include(x => x.Attachment)
                                                                        .Include(x => x.UserTo)
                                                                        .Include(x => x.UserFrom)
                                                                        .OrderByDescending(x => x.DateSent)
                                                                        .FirstOrDefault(x => x.UserFrom.Id == id));
        }
        public PrivateMessage GetMatchingSentPrivateMessage(DateTime date, Guid senderId, Guid receiverId)
        {
            var cacheKey = string.Concat(CacheKeys.PrivateMessage.StartsWith, "GetMatchingSentPrivateMessage-", date.ToString("d"), "-", senderId, "-", receiverId);
            return _cacheService.CachePerRequest(cacheKey, () => _context.PrivateMessage
                                .Include(x => x.UserTo)
                                .Include(x => x.UserFrom)
                                .FirstOrDefault(x => x.DateSent == date && x.UserFrom.Id == senderId && x.UserTo.Id == receiverId && x.IsSentMessage == true));
        }
        public PrivateMessage GetMatchingPrivateMessage(DateTime date, Guid senderId, Guid receiverId)
        {
            var cacheKey = string.Concat(CacheKeys.PrivateMessage.StartsWith, "GetMatchingSentPrivateMessage-", date.ToString("d"), "-", senderId, "-", receiverId);
            return _cacheService.CachePerRequest(cacheKey, () => _context.PrivateMessage
                                .Include(x => x.UserTo)
                                .Include(x => x.UserFrom)
                                .FirstOrDefault(x => x.DateSent == date && x.UserFrom.Id == senderId && x.UserTo.Id == receiverId && x.IsSentMessage == false));
        }

        /// <summary>
        /// Gets all private messages sent by a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<PrivateMessage> GetAllSentByUser(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.PrivateMessage.StartsWith, "GetAllSentByUser-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.PrivateMessage
                                .AsNoTracking()
                                .Include(x => x.UserTo)
                                .Include(x => x.UserFrom)
                                .Where(x => x.UserFrom.Id == id)
                                .OrderByDescending(x => x.DateSent)
                                .ToList());
        }
        public IList<PrivateMessage> GetPrivateMessagesOlderThan(int days)
        {
            var cacheKey = string.Concat(CacheKeys.PrivateMessage.StartsWith, "GetPrivateMessagesOlderThan-", days);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var date = DateTime.UtcNow.AddDays(-days);
                return _context.PrivateMessage.Where(x => x.DateSent <= date).ToList();
            });
        }
        public IList<PrivateMessage> GetNewMessages(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.PrivateMessage.StartsWith, "GetNewMessages-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.PrivateMessage                                                                    
                                                                        .Include(x => x.UserTo)
                                                                        .Include(x => x.UserFrom)                                                                    
                                                                        .Where(x => x.UserTo.Id == id && !x.IsRead)                                
                                                                        .DistinctBy(x => x.UserFrom)
                                                                        .OrderByDescending(x => x.DateSent)
                                                                        .ToList());
        }
        public IList<PrivateMessage> GetAllReceivedByUser(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.PrivateMessage.StartsWith, "GetAllReceivedByUser-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.PrivateMessage
                                                                        .AsNoTracking()
                                                                        .Where(x => x.UserTo.Id == id)
                                                                        .OrderByDescending(x => x.DateSent)
                                                                        .ToList());
        }
        /// <summary>
        /// get all private messages sent from one user to another
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        public IList<PrivateMessage> GetAllByUserToAnotherUser(Guid senderId, Guid receiverId)
        {
            var cacheKey = string.Concat(CacheKeys.PrivateMessage.StartsWith, "GetAllByUserToAnotherUser-", senderId, receiverId);
            return _cacheService.CachePerRequest(cacheKey, () => _context.PrivateMessage
                                                                    .AsNoTracking()
                                                                    .Include(x => x.UserTo)
                                                                    .Include(x => x.UserFrom)
                                                                    .Where(x => x.UserFrom.Id == senderId && x.UserTo.Id == receiverId)
                                                                    .OrderByDescending(x => x.DateSent)
                                                                    .ToList());
        }
        public void DeleteMessage(PrivateMessage message)
        {
           var messageToDelete = _context.PrivateMessage.SingleOrDefault(e => e.Id == message.Id);
         // if (messageToDelete != null)
          //  {
               if (message.IsSentMessage != true)
              {
                   var sentMessage = GetMatchingSentPrivateMessage(message.DateSent, message.UserFrom.Id, message.UserTo.Id);
                  if (sentMessage == null) {
                      var attachementsToDelete = _attachmentService.GetAttachments(message.Id);
                       // var attachementsToDelete = new List<Attachment>();
                      //  attachementsToDelete.AddRange(message.Attachments);
                        foreach (var attachement in attachementsToDelete)
                    {
                          //  message.Attachments.Remove(attachement);
                        // Get File Path
                        var filePath = attachement.FilePath;
                        // Now delete it
                        _attachmentService.Delete(attachement);
                        // And finally delete from the file system
                        System.IO.File.Delete(HostingEnvironment.MapPath(filePath));
                    }
                        message.Attachments.Clear();
               }
            }
              // _context.Entry(message).State = EntityState.Deleted;
            _context.PrivateMessage.Remove(messageToDelete);
           // _context.PrivateMessage.Remove(message);
            _context.SaveChanges();
            //}          
        }
        public IList<PrivateMessage> GetAllSentToUser(Guid senderId, Guid receiverId)
        {
            var messages = _context.PrivateMessage
                  .AsNoTracking()
                  .Include(x => x.UserFrom)
                  .Include(x => x.UserTo)
                 //   .Where(x => (x.UserTo.Id == receiverId && x.UserFrom.Id == senderId && x.IsSentMessage == true) || (x.UserFrom.Id == receiverId && x.UserTo.Id == senderId && x.IsSentMessage != true))
                 .Where(x => (x.UserFrom.Id == senderId && x.UserTo.Id == receiverId && x.IsSentMessage == true))
                  .ToList();
            return messages;
        }
        public IList<PrivateMessage> GetAllReceivedFromUser(Guid senderId, Guid receiverId)
        {
            var messages = _context.PrivateMessage
                  .AsNoTracking()
                  .Include(x => x.UserFrom)
                  .Include(x => x.UserTo)
                 //   .Where(x => (x.UserTo.Id == receiverId && x.UserFrom.Id == senderId && x.IsSentMessage == true) || (x.UserFrom.Id == receiverId && x.UserTo.Id == senderId && x.IsSentMessage != true))
                 .Where(x => (x.UserFrom.Id == senderId && x.UserTo.Id == receiverId && x.IsSentMessage != true))
                  .ToList();
            return messages;
        }
        public PrivateMessage GetLastMessageToUser(Guid senderId, Guid receiverId)
        {
            var cacheKey = string.Concat(CacheKeys.PrivateMessage.StartsWith, "GetLastMessageToUser-",senderId,"-", receiverId);
            return _cacheService.CachePerRequest(cacheKey, () => _context.PrivateMessage
                                                                    // .AsNoTracking()
                                                                        .Include(x => x.UserTo)
                                                                        .Include(x => x.UserFrom)
                                                                        .OrderByDescending(x => x.DateSent)
                                                                        .FirstOrDefault(x => x.UserFrom.Id == senderId && x.UserTo.Id == receiverId));
                                                                       // .OrderBy(x => x.DateSent)
                                                                        //  .Where(x => x.UserFrom.Id == senderId && x.UserTo.Id == receiverId)
                                                                        //  .LastOrDefault());

                                                                       // .LastOrDefault(x => x.UserFrom.Id == senderId && x.UserTo.Id == receiverId));
                                                                     //  .OrderByDescending(x => x.DateSent)
                                                                       // .FirstOrDefault(x => x.UserFrom.Id == id));
        }
    }
}

