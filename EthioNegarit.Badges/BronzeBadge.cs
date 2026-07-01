using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
//awarded to the third-place
namespace EthioNegarit.Badges
{
    [Id("c489d74e-f4e1-429d-a710-b15d013aa99f")]
    [Name("BronzeBadge")]
    [DisplayName("Badge.BronzeBadge.Name")]
    [Description("Badge.BronzeBadge.Desc")]
    [Image("OneThousandPoints.png")]
    [AwardsPoints(10)]
  public  class BronzeBadge : IPostBadge
    {
        public bool Rule(MembershipUser user)
        {
            var postCount = user.Posts.Count;
           // return postCount >= 5;
            return (postCount >= 5);
            //  return postCount >= 10;
        }
    }
}
