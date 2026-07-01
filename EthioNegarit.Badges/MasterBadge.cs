using System.Web.Mvc;
using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using EthioNegarit.Core.Interfaces.Services;

namespace EthioNegarit.Badges
{
    [Id("f06c3c8d-b2c2-4cc6-9a1a-8b3b3c82b9f0")]
    [Name("SolutionMaster")]
    [DisplayName("Badge.SolutionMaster.Name")]
    [Description("Badge.SolutionMaster.Desc")]
    [Image("UserMarkAsSolutionBadge.png")]
    [AwardsPoints(100)]
    public class MasterBadge : ISolutionBadge
    {
        private readonly ITopicService _topicService;
        private readonly ICategoryService _categoryService;

        public MasterBadge()
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
            return _topicService.GetSolvedTopicsByMember(user.Id, allCats).Count >= 100;

        }
    }
}
