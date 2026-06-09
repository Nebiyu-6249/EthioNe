namespace EthioNegarit.WebUI.Controllers
{
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using System.Linq;
    using System.Web.Mvc;
    using ViewModels;
    using System.Collections.Generic;
    using Core.Domain.Enums;
    public partial class TagController : BaseController
    {
        // GET: Tag
        private readonly ITopicTagService _topicTagService;
        private readonly ICategoryService _categoryService;
        public TagController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, ITopicTagService topicTagService, ICategoryService categoryService, ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _topicTagService = topicTagService;
            _categoryService = categoryService;
        }
        [ChildActionOnly]
        public PartialViewResult PopularTags(int amountToTake)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var cacheKey = string.Concat("PopularTags", amountToTake, UsersRole.Id);
                var viewModel = CacheService.Get<PopularTagViewModel>(cacheKey);
                if (viewModel == null)
                {
                    var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                    var popularTags = _topicTagService.GetPopularTags(amountToTake, allowedCategories);
                    viewModel = new PopularTagViewModel { PopularTags = popularTags };
                    CacheService.Set(cacheKey, viewModel, CacheTimes.SixHours);
                }
                return PartialView(viewModel);
            }
        }
        [HttpGet]
        public JsonResult AutoCompleteTags(string term)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var returnList = new List<string>();
                var tags = _topicTagService.GetContains(term);

                if (!tags.Any())
                {
                    return Json(returnList, JsonRequestBehavior.AllowGet);
                }

                foreach (var topicTag in tags)
                {
                    returnList.Add(topicTag.Tag);
                }

                return Json(returnList, JsonRequestBehavior.AllowGet);

            }
        }
    }
}