using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces;
using EthioNegarit.WebUI.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EthioNegarit.WebUI.ViewModels
{
    public class CreatePrivateMessageViewModel
    {
        [UIHint(AppConstants.EditorType), AllowHtml]
       // [Required(ErrorMessage = "Required")]
        [Required]
        public string Message { get; set; }
        [ForumResourceDisplayName("PM.RecipientUsername")]
        [Required]
      //  [HiddenInput]
        public Guid To { get; set; }
        public bool EnableAttachement { get; set; }       
    }
    public class DeletePrivateMessageViewModel
    {
        public Guid Id { get; set; }
    }
    public class DeleteAllMessagesViewModel
    {
        public Guid UserId { get; set;}
     //  public Guid To { get; set; }
    }
    public class GetMoreViewModel
    {
        public Guid UserId { get; set; }
        public int PageIndex { get; set; }
    }

    public class PrivateMessagesViewModel
    {
        public IList<PrivateMessageListItem> AllMessages { get; set; }
		public IList<PrivateMessageListItem> SentMessages{ get; set; }
		public IList<PrivateMessageListItem> ReceivedMessages{ get; set; }
		public IList<PrivateMessageListItem> UnreadMessages{ get; set; }
		public IList<PrivateMessageListItem> StarredMessages{ get; set; }
		public int NewMessagesCount { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class MessagesListViewModel
    {
        public IPagedList<PrivateMessage> PrivateMessages { get; set; }		
		public MembershipUser From { get; set; }
        public bool FromUserIsOnline { get; set; }
        public bool IsAjaxRequest { get; set; }
        public bool IsBlocked { get; set; }
        public bool Blocked { get; set; }
    }

    public class MarkPrivateMessageViewModel
    {
        public Guid Id { get; set; }
    }
   /* public class MessageAttachementsViewModel
    {
    public PrivateMessage Message { get; set; }
    public IList<Attachment> Attachments { get; set; }
    }*/
    public class PrivateMessageAttachementsViewModel
    {
        public PrivateMessage PrivateMessage { get; set; }
        public IList<Attachment> Attachments { get; set; }
        public Guid MessageId { get; set; }
    }
    public class PrivateMessageNotificationViewModel
    {
       // public MembershipUser CurrentUser { get; set; }
        public int NewPrivateMessageCount { get; set; }
        public bool CanViewPrivateMessages { get; set; }
        public IList<PrivateMessage> NewMessages { get; set; }
    }
}