using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using EthioNegarit.Core.Interfaces.Services;
using System.Linq;
using System.Web.Mvc;

namespace EthioNegarit.Badges
{
    [Id("3dbbcc3c-9c5e-48bc-9ce3-388288d954a3")]
    [Name("YourPostFavouritedHundredTimes")]
    [DisplayName("Badge.YourPostFavouritedFiftyTimes.Name")]
    [Description("Badge.YourPostFavouritedFiftyTimes.Desc")]
    [Image("recognised-post.png")]
    [AwardsPoints(50)]
   public class PostFavouritedFiftyTimesBadge : IFavouriteBadge
    {
        private readonly IPostService _postService;

        public PostFavouritedFiftyTimesBadge()
        {
            _postService = DependencyResolver.Current.GetService<IPostService>();
        }

        public bool Rule(MembershipUser user)
        {
            return _postService.GetPostsByFavouriteCount(user.Id, 50).Any();
        }
    }
}
