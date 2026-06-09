using System.Web.Mvc;
using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using EthioNegarit.Core.Interfaces.Services;

namespace EthioNegarit.Badges
{
    [Id("80db5980-ac78-4588-b070-4b634213cce1")]
    [Name("ProblemSolver")]
    [DisplayName("Badge.ProblemSolver.Name")]
    [Description("Badge.ProblemSolver.Desc")]
    [Image("UserMarkAsSolutionBadge.png")]
    [AwardsPoints(10)]
    public class SolverBadge : ISolutionBadge
    {
        private readonly ITopicService _topicService;
        private readonly ICategoryService _categoryService;

        public SolverBadge()
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
            return _topicService.GetSolvedTopicsByMember(user.Id, allCats).Count >= 10;

        }
    }
}
