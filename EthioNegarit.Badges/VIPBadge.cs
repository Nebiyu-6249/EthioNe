using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using System;
using EthioNegarit.Core.Interfaces.Services;
using System.Linq;
using System.Web.Mvc;

namespace EthioNegarit.Badges
{
    [Id("e82a7e0a-4740-42bc-b6ad-57f705466cc7")]
    [Name("VIP")]
    [DisplayName("Badge.VIP.Name")]
    [Description("Badge.VIP.Desc")]
    [Image("recognised-post.png")]
    [AwardsPoints(100)]
   public class VIPBadge :IProfileBadge
    {
        // Year >1
        //point > 1000
        //post count > 100
        private readonly IPostService _postService;

        public VIPBadge()
        {
            _postService = DependencyResolver.Current.GetService<IPostService>();
        }

        public bool Rule(MembershipUser user)
        {
            var anniversary = new DateTime(user.CreateDate.Year + 1, user.CreateDate.Month, user.CreateDate.Day);
            return DateTime.UtcNow >= anniversary;
        }
    }
}
