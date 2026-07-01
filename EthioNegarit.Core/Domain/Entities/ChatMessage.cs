using System;
using EthioNegarit.Utilities;
using System.Collections.Generic;
using System.Linq;
using EthioNegarit.Core.Domain.Enums;

namespace EthioNegarit.Core.Domain.Entities
{
  /*  public enum MessageOrderBy
    {
        Standard,
        SolvedFirst,
        MostReplies,
        MostRecent
    }*/

    public partial class ChatMessage : Entity
    {
        public ChatMessage()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }   
        public bool? FlaggedAsSpam { get; set; }                 
        public bool? IsReplay { get; set; }
     //   public bool? DisableReplay { get; set; }
        public bool? Deleted { get; set; }
        public Guid? InReplyTo { get; set; }       
        public virtual ChatRoom ChatRoom { get; set; }
        public virtual MembershipUser User { get; set; }
        public virtual IList<ChatResponse> ChatResponses { get; set; }
    }
}
