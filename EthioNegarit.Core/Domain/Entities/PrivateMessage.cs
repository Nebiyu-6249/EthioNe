using System;
using EthioNegarit.Utilities;
using EthioNegarit.Core.Domain.General;
using System.Collections.Generic;
using System.Web;

namespace EthioNegarit.Core.Domain.Entities
{
    public partial class PrivateMessage : Entity
    {
        
         public PrivateMessage()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public DateTime DateSent { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public bool? IsSentMessage { get; set; }
        public bool IsStarred { get; set; }
        public virtual MembershipUser UserTo { get; set; }
        public virtual MembershipUser UserFrom { get; set; }
       public virtual IList<Attachment> Attachments { get; set; }
      
    }
  /*  public enum MessageShow
    {
        Received,
        Sent,
        Starred
    };*/
    public partial class PrivateMessageListItem
    {
        public MembershipUser User { get; set; }
        public DateTime Date { get; set; }
        public bool HasUnreadMessages { get; set; }

        public bool IsSentMessage { get; set; }

        public bool IsReceivedMessage { get; set; }
       
        public bool UnRead { get; set; }

        public bool IsStarred { get; set; }

        public int AttachmentCount { get; set; }
        public bool Hasattachments { get; set; }

        public string Message { get; set; }
        public Guid MessageId { get; set; }
      //  public PrivateMessage PrivateMessage { get; set; }
        
    }
    }
