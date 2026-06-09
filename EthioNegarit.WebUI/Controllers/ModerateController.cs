namespace EthioNegarit.WebUI.Controllers
{
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Core.Constants;
    using System;
    using System.Web.Mvc;
    using ViewModels;
    using Core.Domain.Entities;
    using System.Collections.Generic;

    public partial class ModerateController : BaseController
    {
        private readonly IPostService _postService;
        private readonly ITopicService _topicService;
        private readonly ICategoryService _categoryService;

        public ModerateController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, IPostService postService,
            ITopicService topicService, ICategoryService categoryService, ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _postService = postService;
            _topicService = topicService;
            _categoryService = categoryService;
        }
        public ActionResult Index()
        {
            // Show both pending topics and also pending posts
            // Use ajax for paging too
        //    List<Topic> topicsList = new List<Topic>();
            var viewModel = new ModerateViewModel();
            if (LoggedOnReadOnlyUser != null)
            {               
                if (User.IsInRole(AppConstants.AdminRoleName)) {
                    viewModel.Posts = _postService.GetAllPendingPosts();
                    viewModel.Topics = _topicService.GetAllPendingTopics();
                }
                else
                {
                    var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                    viewModel.Topics = _topicService.GetPendingTopics(allowedCategories, UsersRole, LoggedOnReadOnlyUser.Id);
                    viewModel.Posts = _postService.GetPendingPosts(allowedCategories, UsersRole, LoggedOnReadOnlyUser.Id);
                }
            }
          
            if (Request.IsAjaxRequest())
            {
                viewModel.IsAjaxRequest = true;
            }
            else
            {
                viewModel.IsAjaxRequest = false;
            }
                return View(viewModel);
        }

        [HttpPost]
        public ActionResult ModerateTopic(ModerateActionViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var topic = _topicService.Get(viewModel.TopicId);
                    var permissions = RoleService.GetPermissions(topic.Category, UsersRole);

                    // Is this user allowed to moderate - We use EditPosts for now until we change the permissions system
                    if (!permissions[SiteConstants.Instance.PermissionEditPosts].IsTicked)
                    {
                        return Content(LocalizationService.GetResourceString("Errors.NoPermission"));
                    }

                    if (viewModel.IsApproved)
                    {
                        topic.Pending = false;
                    }
                    else
                    {
                        _topicService.Delete(topic, unitOfWork);
                    }

                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    return Content(ex.Message);
                }
            }

            return Content("allgood");
        }

        [HttpPost]
        public ActionResult ModeratePost(ModerateActionViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {

                try
                {
                    var post = _postService.Get(viewModel.PostId);
                    var permissions = RoleService.GetPermissions(post.Topic.Category, UsersRole);
                    if (!permissions[SiteConstants.Instance.PermissionEditPosts].IsTicked)
                    {
                        return Content(LocalizationService.GetResourceString("Errors.NoPermission"));
                    }

                    if (viewModel.IsApproved)
                    {
                        post.Pending = false;
                    }
                    else
                    {
                        _postService.Delete(post, unitOfWork, false);
                    }

                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    return Content(ex.Message);
                }
            }

            return Content("allgood");
        }

    }
}