using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using System.Linq;
namespace EthioNegarit.Badges
{
    [Id("c9913ee2-b8e0-4543-8930-c723497ee65c")]
    [Name("OneThousandPoints")]
    [DisplayName("Badge.OneThousandPoints.Name")]
    [Description("Badge.OneThousandPoints.Desc")]
    [Image("OneThousandPoints.png")]
    [AwardsPoints(10)]
   public class OneThousandPoints : IPostBadge
    {
        public bool Rule(MembershipUser user)
        {
            var points = user.Points.Sum(x => x.Points);
            return points >= 1000;
        }
    }
}
