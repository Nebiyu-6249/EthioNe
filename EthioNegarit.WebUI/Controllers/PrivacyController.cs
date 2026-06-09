using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using System.Web.Mvc;
using EthioNegarit.WebUI.ViewModels;
using EthioNegarit.Utilities;
using System;

namespace EthioNegarit.WebUI.Controllers
{
    public partial class PrivacyController : BaseController
    {
        private readonly IPrivacyPolicyService _privacyPolicyService;
        public PrivacyController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, ILocalizationService localizationService, IMembershipService membershipService, IRoleService roleService, ICacheService cacheService, ISettingsService settingsService, IPrivacyPolicyService privacyPolicyService)
               : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _privacyPolicyService = privacyPolicyService;
          }
        public ActionResult Index()
        {
            var language = (LocalizationService.CurrentLanguage.LanguageCulture);
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                var policy = _privacyPolicyService.GetPolicy();
                var date = policy.LastUpdate.ToString();
                var viewModel = new PrivacyPolicyViewModel();
                viewModel.Version = policy.Version;
                viewModel.EffectiveDate = DateUtils.FormatDateTime(date, "MMMM, dd yyyy");
                if (language == "am-ET")
                {
                    viewModel.PrivacyPolicy = policy.PolicyAm;
                }
                else
                {
                    viewModel.PrivacyPolicy = policy.Policy;
                }
                return View(viewModel);
            }
        }
        public ActionResult Terms()
        {
            var language = (LocalizationService.CurrentLanguage.LanguageCulture);
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                var policy =   _privacyPolicyService.GetPolicy();
                var date = policy.LastUpdate.ToString();
                var viewModel = new PrivacyPolicyViewModel();
                viewModel.Version = policy.Version;
                viewModel.EffectiveDate = DateUtils.FormatDateTime(date, "MMMM, dd yyyy");
                if (settings.AgreeToTermsAndConditions == true && LoggedOnReadOnlyUser != null)
                {
                    viewModel.Agree = (bool)LoggedOnReadOnlyUser.HasAgreedToTermsAndConditions;
                }

                if (language == "am-ET")
                {
                    viewModel.TermsAndConditions = policy.TermsAm;
                }
                else
                {
                    viewModel.TermsAndConditions = policy.Terms;
                }
                return View(viewModel);
            }
        }
        public ActionResult Agreement()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                var viewModel = new AgreeToTermsAndConditionsViewModel
                {
                    Agree = false,
                   // TermsAndConditions = settings.TermsAndConditions
                };
                return View(viewModel);
            }
        }
        [HttpPost]
        public ActionResult Agreement(AgreeToTermsAndConditionsViewModel viewmodel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    var user = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    user.HasAgreedToTermsAndConditions = viewmodel.Agree;
                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                    return RedirectToAction("Index","Home");
                }
            }

            return View(viewmodel);
        }
    }
}