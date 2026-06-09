using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IChatService
    {
        ChatRoom SanitizeChatRoom(ChatRoom chatRoom);
        ChatRoom Get(Guid id);
        ChatRoom GetBySlug(string slug);
        IList<ChatRoom> GetBySlugLike(string slug);
        IList<ChatRoom> GetChatRooms();
        ChatRoom Add(ChatRoom chatRoom);
        void UpdateSlugFromName(ChatRoom chatRoom);
        void Delete(ChatRoom chatRoom);
        ChatRoom GetChatRoom(Guid id);
        ChatRoom GetChatRoom(string slug); 
        ChatMessage GetMessage(Guid id);
        ChatMessage AddMessage(ChatMessage chatMessage);
        void Delete(ChatMessage message);        
        IList<ChatRoomMember> GetRoomSubscription(MembershipUser user, ChatRoom chatRoom, bool addTracking = false);
        ChatRoomMember AddMember(ChatRoomMember chatRoomMember);    
        ChatRoomMember GetRoomSubscription(Guid userId, Guid chatRoomId);
        IList<ChatResponse> GetResponses(List<Guid> messageIds);
        void DeleteResponse(ChatResponse chatResponse);
        ChatResponse AddResponse(ChatResponse chatResponse);
        IList<ChatRoomMember> GetRoomMembers(int amountToTake, Guid chatRoomId);
        IList<ChatRoom> GetAll();  
        void DeleteSubscription(ChatRoomMember chatRoomMember);
        PagedList<ChatRoomMember> GetRoomMembers(Guid chatRoomId, int pageIndex, int pageSize, int amountToTake);
        IList<ChatMessage> GetReplies(Guid messageId);
        IList<ChatResponse> GetReports(Guid userId, DateTime? from);   
        PagedList<ChatMessage> GetRoomMessages(int pageIndex, int pageSize, int amountToTake, Guid chatRoomId);
        PagedList<ChatMessage> GetUserMessages(int pageIndex, int pageSize, int amountToTake, Guid chatRoomId, Guid userId);
    }
}
