using System.Web.Mvc;
using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using EthioNegarit.Core.Interfaces.Services;

namespace EthioNegarit.Badges
{
    [Id("04030201-0605-0807-090a-0b0c0d0e0f10")]
    [Name("SolutionFinder")]
    [DisplayName("Badge.SolutionFinder.Name")]
    [Description("Badge.SolutionFinder.Desc")]
    [Image("UserMarkAsSolutionBadge.png")]
    [AwardsPoints(50)]
    public class StarBadge : ISolutionBadge
    {
        private readonly ITopicService _topicService;
        private readonly ICategoryService _categoryService;

        public StarBadge()
        {
            _topicService = DependencyResolver.Current.GetService<ITopicService>();
            _categoryService = DependencyResolver.Current.GetService<ICategoryService>();
        }

        /// <summary>
        /// Post is marked as the answer to a topic - give the topic author a badge
        /// </summary>
        /// <returns></returns>
        public bool Rule(MembershipUser user)
        {
            var allCats = _categoryService.GetAll();

            //var usersSolutions = _postService.GetSolutionsByMember(user.Id, cats);
            // return (usersSolutions.Count >= 10);
            return _topicService.GetSolvedTopicsByMember(user.Id, allCats).Count >= 50;

        }
    }
}
