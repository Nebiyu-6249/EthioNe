using System;
using EthioNegarit.Utilities;

namespace EthioNegarit.Core.Domain.Entities
{
    public partial class ChatRoomMember : Entity
    {
        public ChatRoomMember()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public DateTime LastActive { get; set; }
        public bool EnableNotification { get; set; }
        public bool LogOn { get; set; }
        public virtual ChatRoom ChatRoom { get; set; }
        public virtual MembershipUser User { get; set; }
        
    }
}
