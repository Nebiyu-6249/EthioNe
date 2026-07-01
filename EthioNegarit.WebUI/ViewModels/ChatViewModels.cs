using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace EthioNegarit.WebUI.ViewModels
{
    public class ChatRoomsViewModel
        {
        public List<ChatRoomViewModel> ChatRooms { get; set; }
        public int PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public bool Freezed { get; set; }
        public int FreezedRoomsCount { get; set; }
    }
    public class ChatRoomViewModel
        {
        public ChatRoom ChatRoom { get; set; }
        // Subscription
        public bool IsSubscribed { get; set; }
        }
    public class ChatMessagesViewModel
        {
        public ChatRoom ChatRoom { get; set; }
        public List<ChatMessageViewModel> ChatMessages { get; set; }
        public int PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public Guid RoomId { get; set; }
    }
    public class ChatMessageViewModel
        {
        public ChatMessage ChatMessage { get; set; }
        public List<ChatResponse> ChatResponses { get; set; }
        public MembershipUser User { get; set; }
        public bool AllowedToResponse { get; set; }
        public bool EnableDelete { get; set; }
        public bool EnableReplay { get; set; }
        public bool MemberIsOnline { get; set; }
        public int Replies { get; set; }
       // public bool LogOn { get; set; }
        public bool HasReport { get; set; }
        public bool HasFavourite { get; set; }
    }
    public class CreateMessageViewModel
        {
        [Required(ErrorMessage = "Say Something")]
        [StringLength(6000, MinimumLength = 2)]
        public string Message { get; set; }
        public Guid RoomId { get; set; }
        public bool? IsReplay { get; set; }
        public Guid? InReplyTo { get; set; }
        public string ReplyToUsername { get; set; }
        //For Private Message
        public Guid? ReplyToUserId { get; set; }       
        public ChatRoom ChatRoom { get; set; }
        public MembershipUser User { get; set; }
    }
    public class ChatResponseViewModel
        {
        public Guid MessageId { get; set; }
        }
    public class ChatRoomMembersViewModel
        {
        public List<ChatRoomMember> ChatRoomMembers { get; set; }
       public List<MembershipUser> RoomMembers { get; set; }
        public string Search { get; set; }
        public int? PageIndex { get; set; }
        public ChatRoom ChatRoom { get; set; }
        public bool MemberIsOnline { get; set; }
    }
    public class ChatModel
        {
        public ChatRoom ChatRoom { get; set; }
        public bool IsSubscribed { get; set; }
        public bool chatEnabled { get; set; }
        public bool LogOn { get; set; }        
        public List<ChatUser> Users;
        public Guid UserId { get; set; }
        public ChatModel ()
            {
            Users = new List<ChatUser>();
            //   ChatHistory = new List<ChatMessage>();
            /* ChatHistory.Add(new ChatMessage()
             {
                 Message = "The chat server started at " + DateTime.Now
             });*/
            }
        public class ChatUser
            {
            public MembershipUser User { get; set; }
            public DateTime LoggedOnTime;
            public DateTime LastPing;
            }
        }
    public class DeleteChatMessageViewModel
    {
         public Guid Id{ get; set; }
    }
}