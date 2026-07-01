using System;
using EthioNegarit.Utilities;

namespace EthioNegarit.Core.Domain.Entities
{
    public partial class ChatResponse : Entity
    {
        public ChatResponse()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public virtual ChatMessage ChatMessage { get; set; }
        public virtual MembershipUser User { get; set; }    
        public virtual MembershipUser MembershipUserRespond { get; set; }
        public virtual DateTime DateResponse { get; set; }
    }
}
