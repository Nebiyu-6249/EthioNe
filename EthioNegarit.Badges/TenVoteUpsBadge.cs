using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using System;
using System.Linq;
namespace EthioNegarit.Badges
{
    [Id("0f8fad5b-d9cb-469f-a165-70867728950e")]
    [Name("TenVoteUps")]
    [DisplayName("Badge.TenVoteUps.Name")]
    [Description("Badge.TenVoteUps.Desc")]
    [Image("PosterVoteUpBadge.png")]
    [AwardsPoints(10)]
   public class TenVoteUpsBadge : IVoteUpBadge
    {
        public bool Rule(MembershipUser user)
        {
            return user.Posts != null && user.Posts.Any(post => post.Votes.Count > 10);
        }
    }
}
