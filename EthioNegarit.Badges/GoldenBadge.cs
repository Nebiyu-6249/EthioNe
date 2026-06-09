using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using EthioNegarit.Core.Interfaces.Services;
using System.Linq;
using System.Web.Mvc;

namespace EthioNegarit.Badges
{
    [Id("a54ec5d1-111d-4698-b2d0-78fbdaa52d1b")]
    [Name("GoldenBadge")]
    [DisplayName("Badge.OneThousandPoints.Name")]
    [Description("Badge.OneThousandPoints.Desc")]
    [Image("OneThousandPoints.png")]
    [AwardsPoints(100)]

    
   public class GoldenBadge : IPostBadge
    {
        public bool Rule(MembershipUser user)
        {
            var postCount = user.Posts.Count;
          
            return (postCount >= 15);
            // return postCount >= 100;
        }
    }
}
