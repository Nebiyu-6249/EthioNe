namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Web.Mvc;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Areas.Admin.ViewModels;
    public class NotificationController : BaseController
    {
        private readonly ITopicService _topicService;
        private readonly IPostService _postService;
        private readonly IFavouriteService _favouriteService;
        private readonly ICategoryService _categoryService;
        private readonly IPrivateMessageService _privateMessageService;
        public NotificationController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            IRoleService roleService, ITopicService topicService, IPostService postService,
            ILocalizationService localizationService, ISettingsService settingsService,
            IFavouriteService favouriteService, ICacheService cacheService, ICategoryService categoryService, IPrivateMessageService privateMessageService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _topicService = topicService;
            _postService = postService;
            _favouriteService = favouriteService;
            _categoryService = categoryService;
            _privateMessageService = privateMessageService;
        }
        [HttpPost]
        [Authorize]
        public void Update()
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    loggedOnUser.NotificationLastChecked = DateTime.UtcNow;
                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            else
            {
                throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }
        }      
        public virtual void Desktop()
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Get a db user
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                        if (loggedOnUser.DesktopNotification == false)
                        {
                            loggedOnUser.DesktopNotification = true;
                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Desktop Notification Enabled"),
                                MessageType = GenericMessages.info
                            });
                        }
                        else
                        {
                            loggedOnUser.DesktopNotification = false;
                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Desktop Notification Disabled"),
                                MessageType = GenericMessages.info
                            });
                        }
                       
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
        }
      public virtual void Email()
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Get a db user
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);                 
                        if (loggedOnUser.DisableEmailNotifications == false)
                        {
                            loggedOnUser.DisableEmailNotifications = true;
                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Email Notification Enabled"),
                                MessageType = GenericMessages.info
                            });
                        }
                        else
                        {
                            loggedOnUser.DisableEmailNotifications = false;
                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Email Notification Disabled"),
                                MessageType = GenericMessages.info
                            });
                        }
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
        }  
    }
}
