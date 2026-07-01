using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using EthioNegarit.Core.Interfaces.Services;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
//awarded to the 2nd-place
namespace EthioNegarit.Badges
{
    [Id("aacd26a9-be3e-4fe1-b0bf-b15d013aa9ac")]
    [Name("SilverBadge")]
    [DisplayName("Badge.SilverBadge.Name")]
    [Description("Badge.SilverBadge.Desc")]
    [Image("OneThousandPoints.png")]
    [AwardsPoints(50)]
    public class SilverBadge : IPostBadge
    {
        public bool Rule(MembershipUser user)
        {
            var postCount = user.Posts.Count;
            //  return postCount >= 50;
            return (postCount >= 10);
           // return postCount >= 10;
        }
    }
}
