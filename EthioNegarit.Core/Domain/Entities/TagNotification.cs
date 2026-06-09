using System;
using EthioNegarit.Utilities;

namespace EthioNegarit.Core.Domain.Entities
{
    public partial class TagNotification : Entity
    {
           public TagNotification()
           {
               Id = GuidComb.GenerateComb();
           }
           public Guid Id { get; set; }
           public virtual TopicTag Tag { get; set; }
           public virtual MembershipUser User { get; set; }
           
    }
}
