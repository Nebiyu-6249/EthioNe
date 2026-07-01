using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IPrivateMessageService
    {       
        PrivateMessage SanitizeMessage(PrivateMessage privateMessage);
        PrivateMessage Add(PrivateMessage message);
        PrivateMessage Get(Guid id);
        IPagedList<PrivateMessageListItem> GetUserPrivateMessages(int pageIndex, int pageSize, MembershipUser user);
        IPagedList<PrivateMessage> GetUsersConversation(int pageIndex, int pageSize, MembershipUser toUser, MembershipUser fromUser);
        PrivateMessage GetLastSentPrivateMessage(Guid id);
        PrivateMessage GetMatchingPrivateMessage(DateTime date, Guid senderId, Guid receiverId);
        PrivateMessage GetMatchingSentPrivateMessage(DateTime date, Guid senderId, Guid receiverId);
        IList<PrivateMessage> GetAllSentByUser(Guid id);
        /// <summary>
        /// Get a list of private messages older than X days
        /// </summary>
        /// <param name="days">Amount of days to go back</param>
        /// <returns></returns>
        IList<PrivateMessage> GetPrivateMessagesOlderThan(int days);
       // int NewPrivateMessageCount(Guid userId);
        IList<PrivateMessage> GetAllReceivedByUser(Guid id);
        IList<PrivateMessage> GetAllByUserToAnotherUser(Guid senderId, Guid receiverId);     
        void DeleteMessage(PrivateMessage message);
        IList<PrivateMessage> GetNewMessages(Guid id);
        IList<PrivateMessage> GetAllSentToUser(Guid senderId, Guid receiverId);
        IList<PrivateMessage> GetAllReceivedFromUser(Guid senderId, Guid receiverId);
        PrivateMessage GetLastMessageToUser(Guid senderId, Guid receiverId);
    }
}
