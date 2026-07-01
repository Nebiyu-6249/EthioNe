using EthioNegarit.Utilities;
using System;
using System.Collections.Generic;


namespace EthioNegarit.Core.Domain.Entities
{
    public partial class Poll : Entity
    {
         public Poll()
         {
             Id = GuidComb.GenerateComb();
         }
         public Guid Id { get; set; }
         public bool IsClosed { get; set; }
         public DateTime DateCreated { get; set; }
         public int? ClosePollAfterDays { get; set; }

         public virtual MembershipUser User { get; set; }
         public virtual IList<PollAnswer> PollAnswers { get; set; }
         
    }
}
