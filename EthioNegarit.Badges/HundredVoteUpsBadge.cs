using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using System;
using System.Linq;
namespace EthioNegarit.Badges
{
    [Id("e42363f8-ec38-42dd-aa46-a677d52e743c")]
    [Name("HundredVoteUps")]
    [DisplayName("Badge.HundredVoteUps.Name")]
    [Description("Badge.HundredVoteUps.Desc")]
    [Image("PosterVoteUpBadge.png")]
    [AwardsPoints(100)]
   public class HundredVoteUpsBadge : IVoteUpBadge
    {
        public bool Rule(MembershipUser user)
        {
            return user.Posts != null && user.Posts.Any(post => post.Votes.Count > 100);
        }
    }
}
