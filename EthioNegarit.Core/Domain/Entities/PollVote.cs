using System;
using EthioNegarit.Utilities;

namespace EthioNegarit.Core.Domain.Entities
{
    public partial class PollVote
    {
             public PollVote()
             {
                 Id = GuidComb.GenerateComb();
             }

             public Guid Id { get; set; }
             public virtual PollAnswer PollAnswer { get; set; }
             public virtual MembershipUser User { get; set; }
             
    }
}
