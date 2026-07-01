namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Web.Mvc;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Areas.Admin.ViewModels;
    public class NotificationController : BaseController
    {
        public NotificationController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            IRoleService roleService,ILocalizationService localizationService, ISettingsService settingsService,ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
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
