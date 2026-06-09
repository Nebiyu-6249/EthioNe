using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using EthioNegarit.Core.Interfaces.Services;
using System.Linq;
using System.Web.Mvc;

namespace EthioNegarit.Badges
{
    [Id("C34784C1-FA77-4A0A-8141-9762A4069961")]
    [Name("YourPostFavouritedTenTimes")]
    [DisplayName("Badge.YourPostFavouritedTenTimes.Name")]
    [Description("Badge.YourPostFavouritedTenTimes.Desc")]
    [Image("recognised-post.png")]
    [AwardsPoints(10)]
    public class PostFavouritedTenTimesBadge : IFavouriteBadge
    {
        private readonly IPostService _postService;

        public PostFavouritedTenTimesBadge()
        {
            _postService = DependencyResolver.Current.GetService<IPostService>();
        }

        public bool Rule(MembershipUser user)
        {
            return _postService.GetPostsByFavouriteCount(user.Id, 10).Any();
        }
    }
}
