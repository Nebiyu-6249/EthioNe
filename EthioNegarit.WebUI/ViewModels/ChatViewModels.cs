using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EthioNegarit.WebUI.ViewModels
    {
    public class ChatRoomsViewModel
        {
        public IEnumerable<ChatRoomViewModel> ChatRooms { get; set; }
        }

    public class ChatRoomViewModel
        {
        public ChatRoom ChatRoom { get; set; }
        public List<ChatMessageViewModel> ChatMessages { get; set; }
        // Permissions
        public bool DisableChat { get; set; }

        // Subscription
        public bool IsSubscribed { get; set; }
        public bool ShowUnSubscribedLink { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }

        }
   /* public class ShowPreviousMessagesViewModel
        {
        public List<ChatMessageViewModel> ChatMessages { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public ChatRoom ChatRoom { get; set; }
        }*/
   /* public class ShowMoreMessagesViewModel
        {
        public List<ChatMessageViewModel> ChatMessages { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public ChatRoom ChatRoom { get; set; }
        }*/

    public class ChatMessagesViewModel
        {
        public ChatRoom ChatRoom { get; set; }
     //   public bool IsLive { get; set; }
        public bool LogOn { get; set; }
        public bool ChatEnabled { get; set; }
        public List<ChatMessageViewModel> ChatMessages { get; set; }
      //  public int? PageIndex { get; set; }
        public int PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public bool MyChat { get; set; }
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
        public bool LogOn { get; set; }
        public bool HasReport { get; set; }
        public bool HasFavourite { get; set; }
    }

    public class CreateMessageViewModel
        {
        // [RegularExpression(User.EmailRegularExpression, ErrorMessage = "Введите корректный адрес электронной почты")]
        //   [Required(ErrorMessage = "Required")]
        //    [UIHint(AppConstants.EditorType), AllowHtml]
        //   [StringLength(6000)]
        // [DataType(DataType.Date)]
        //  [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyy-MM-dd}")]
        //  [Display(Name = "Deleted on:")]
      //  [StringLength(6000, MinimumLength = 5, ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        [Required(ErrorMessage = "Say Something")]

        [StringLength(6000, MinimumLength = 5)]
        public string Message { get; set; }
       // public string Content { get; set; }
        public Guid RoomId { get; set; }
        public bool? IsReplay { get; set; }
        public Guid? InReplyTo { get; set; }
        public string ReplyToUsername { get; set; }
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
        //active Members count
        //all members count
    }



    public class ChatModel
        {
        /// <summary>
        /// Users that have connected to the chat
        /// </summary>
        /// 
        public bool IsSubscribed { get; set; }
        public bool chatEnabled { get; set; }
        public bool LogOn { get; set; }
        public ChatRoom ChatRoom { get; set; }
        public List<ChatUser> Users;

        public Guid UserId { get; set; }
        // public List<ChatMessage> ChatHistory;

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
            // public string NickName;
            public DateTime LoggedOnTime;
            public DateTime LastPing;
            }
        }
   /* public class GetMoreMessagesViewModel
        {
        public Guid RoomId { get; set; }
        public int PageIndex { get; set; }
        public bool MyChat { get; set; }
    }*/

    public class DeleteChatMessageViewModel
    {
         public Guid Id{ get; set; }
    }
    // Sent to the realtime chat client by ChatHub. Property names are forced to
    // camelCase so the JavaScript reads msg.id / msg.content / msg.authorName etc.
    public class MessageViewModel
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public Guid Id { get; set; }                 // was int — ChatMessage.Id is a Guid

        [Newtonsoft.Json.JsonProperty("content")]
        public string Content { get; set; }          // maps from ChatMessage.Content

        [Newtonsoft.Json.JsonProperty("authorName")]
        public string AuthorName { get; set; }

        [Newtonsoft.Json.JsonProperty("authorId")]
        public Guid AuthorId { get; set; }

        [Newtonsoft.Json.JsonProperty("gravatarHash")]
        public string GravatarHash { get; set; }

        [Newtonsoft.Json.JsonProperty("sentAt")]
        public string SentAt { get; set; }           // ISO-8601 string

        [Newtonsoft.Json.JsonProperty("starCount")]
        public int StarCount { get; set; }

        [Newtonsoft.Json.JsonProperty("isStarred")]
        public bool IsStarred { get; set; }          // by the current user

        [Newtonsoft.Json.JsonProperty("parentId")]
        public Guid? ParentId { get; set; }          // was int? — InReplyTo is a Guid?

        [Newtonsoft.Json.JsonProperty("parentPreview")]
        public string ParentPreview { get; set; }

        [Newtonsoft.Json.JsonProperty("parentAuthor")]
        public string ParentAuthor { get; set; }
    }
}
























/* public class ChatRoomListViewModel
 {
     public IEnumerable<ChatRoom> ChatRooms { get; set; }
     //  public List<ChatRoomViewModel> ChatRooms { get; set; }
 }*/


















/*
public class ChatModel
{

    /// <summary>
    /// Users that have connected to the chat
    /// </summary>
    public List<ChatUser> Users;

    /// <summary>
    /// Messages by the users
    /// </summary>
    public List<ChatMessage> ChatHistory;

    public ChatModel()
    {
        Users = new List<ChatUser>();
        ChatHistory = new List<ChatMessage>();

        ChatHistory.Add(new ChatMessage()
        {
            Message = "The chat server started at " + DateTime.Now
        });
    }

    public class ChatUser
    {
        public MembershipUser User { get; set; }
        public string NickName;
        public DateTime LoggedOnTime;
        public DateTime LastPing;
    }

    public class ChatMessage
    {
        /// <summary>
        /// If null, the message is from the server
        /// </summary>
        public ChatUser ByUser;

        public DateTime When = DateTime.Now;

        public string Message = "";

    }
}
*/

/*
   public class ChatViewModel
{

    /// <summary>
    /// Users that have connected to the chat
    /// </summary>
    /// 
    public bool IsSubscribed { get; set; }
    public bool LogOn { get; set; }
    public ChatRoom ChatRoom { get; set; }
    public List<ChatUser> Users;

    public Guid UserId { get; set; }      
   // public List<ChatMessage> ChatHistory;

    public ChatViewModel()
    {
        Users = new List<ChatUser>();
     //   ChatHistory = new List<ChatMessage>();
       /* ChatHistory.Add(new ChatMessage()
        {
            Message = "The chat server started at " + DateTime.Now
        });*//*
    }

    public class ChatUser
{
public MembershipUser User { get; set; }
//public Guid GroupId { get; set; }
// public string NickName;
public DateTime LoggedOnTime;
public DateTime LastPing;
}

  /*  public class ChatMessage
    {
        /// <summary>
        /// If null, the message is from the server
        /// </summary>
        public MembershipUser User { get; set; }
        public ChatUser ByUser;
        public DateTime When = DateTime.Now;
        public string Message = "";
    }*//*
}


*/
