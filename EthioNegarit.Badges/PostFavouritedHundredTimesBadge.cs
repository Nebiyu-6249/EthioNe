using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using EthioNegarit.Core.Interfaces.Services;
using System.Linq;
using System.Web.Mvc;

namespace EthioNegarit.Badges
{
    [Id("362add4b-2bf6-4f6c-9d21-cef73e478682")]
    [Name("YourPostFavouritedHundredTimes")]
    [DisplayName("Badge.YourPostFavouritedHundredTimes.Name")]
    [Description("Badge.YourPostFavouritedHundredTimes.Desc")]
    [Image("recognised-post.png")]
    [AwardsPoints(100)]
   public class PostFavouritedHundredTimesBadge : IFavouriteBadge
    {
        private readonly IPostService _postService;

        public PostFavouritedHundredTimesBadge()
        {
            _postService = DependencyResolver.Current.GetService<IPostService>();
        }

        public bool Rule(MembershipUser user)
        {
            return _postService.GetPostsByFavouriteCount(user.Id, 100).Any();
        }
    }
}
