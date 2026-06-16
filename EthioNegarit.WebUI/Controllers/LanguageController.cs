namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Linq;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using ViewModels;
    using Core.Constants;
    using System.Web;
    using Areas.Admin.ViewModels;
    public partial class LanguageController : BaseController
    {
        public LanguageController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ILocalizationService localizationService,
             IRoleService roleService, ISettingsService settingsService, ICacheService cacheService)
             : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {

        }

        [ChildActionOnly]
        public ActionResult Index()
        {
            var viewModel = new LanguageListAllViewModel
            {
                Alllanguages = LocalizationService.AllLanguages,
                CurrentLanguage = LocalizationService.CurrentLanguage.Id
            };
            if (viewModel.Alllanguages.Count() <= 1)
            {
                return Content(string.Empty);
            }
            return PartialView("_LanguagePartial", viewModel);
        }
        [HttpPost]
        public ActionResult ChangeLanguage(Guid lang)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var language = LocalizationService.Get(lang);
                LocalizationService.CurrentLanguage = language;
                // The current language is stored in a cookie
                var cookie = new HttpCookie(AppConstants.LanguageIdCookieName)
                {
                    HttpOnly = false,
                    Value = language.Id.ToString(),
                    Expires = DateTime.UtcNow.AddYears(1)
                };
                Response.Cookies.Add(cookie);
               /* TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("Language.Changed"),
                    MessageType = GenericMessages.success
                };*/
                return Content("success");
            }
        }
    }
}