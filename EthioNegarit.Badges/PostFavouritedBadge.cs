using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using EthioNegarit.Core.Interfaces.Services;
using System.Linq;
using System.Web.Mvc;

namespace EthioNegarit.Badges
{
    [Id("6EF03C66-9094-40B6-9F60-10065BF89104")]
    [Name("YourPostFavourited")]
    [DisplayName("Badge.YourPostFavourited.Name")]
    [Description("Badge.YourPostFavourited.Desc")]
    [Image("your-post-got-favourited-by-another-member.png")]
    [AwardsPoints(2)]
   public class PostFavouritedBadge : IFavouriteBadge
    {
        private readonly IPostService _postService;

        public PostFavouritedBadge()
        {
            _postService = DependencyResolver.Current.GetService<IPostService>();
        }


        public bool Rule(MembershipUser user)
        {
            return _postService.GetPostsFavouritedByOtherMembers(user.Id).Any();
        }
    }
}
