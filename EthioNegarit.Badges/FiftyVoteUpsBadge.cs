using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using System;
using System.Linq;
namespace EthioNegarit.Badges
{
    [Id("80db5980-ac78-4588-b070-4b634213cce1")]
    [Name("FiftyVoteUps")]
    [DisplayName("Badge.FiftyVoteUps.Name")]
    [Description("Badge.FiftyVoteUps.Desc")]
    [Image("PosterVoteUpBadge.png")]
    [AwardsPoints(50)]
  public  class FiftyVoteUpsBadge : IVoteUpBadge
    {
        public bool Rule(MembershipUser user)
        {
            return user.Posts != null && user.Posts.Any(post => post.Votes.Count > 50);
        }
    }
}
