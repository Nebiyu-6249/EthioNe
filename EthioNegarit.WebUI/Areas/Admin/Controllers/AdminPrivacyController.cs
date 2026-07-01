using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.Utilities;
using EthioNegarit.WebUI.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthioNegarit.WebUI.Areas.Admin.Controllers
{
    public class AdminPrivacyController : BaseAdminController
    {
        private readonly IPrivacyPolicyService _privacyPolicyService;

        public AdminPrivacyController(ILoggingService loggingService,IUnitOfWorkManager unitOfWorkManager,IMembershipService membershipService,ILocalizationService localizationService, IPrivacyPolicyService privacyPolicyService, ISettingsService settingsService)
            : base(loggingService, unitOfWorkManager, membershipService, localizationService, settingsService)
        {
            _privacyPolicyService = privacyPolicyService;
        }
        public ActionResult Index()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var policy = _privacyPolicyService.GetPolicy();
                var policyViewModel = CreateEditPolicyViewModel(policy);

                return View(policyViewModel);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(EditPrivacyPolicyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var policy = _privacyPolicyService.GetPolicy(false);
                      //  policy.Id = viewModel.Id;
                        // policy.LastUpdate = viewModel.LastUpdate;
                        policy.Version = viewModel.Version;
                        policy.LastUpdate = DateTime.UtcNow;
                        policy.EffectiveDate = viewModel.EffectiveDate;
                        policy.Policy = viewModel.Policy;
                        policy.PolicyAm = viewModel.PolicyAm;
                        policy.Terms = viewModel.Terms;
                        policy.TermsAm = viewModel.TermsAm;
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Privacy Policy Successfully Updated",
                            MessageType = GenericMessages.success
                        };
                        unitOfWork.Commit();
                      //  viewModel = CreateEditPolicyViewModel(policy);                       
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Privacy Policy Update Failed",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
            }
            return View();
          //  return View(viewModel);
        }
        private EditPrivacyPolicyViewModel CreateEditPolicyViewModel(PrivacyPolicy privacyPolicy)
        {
            var lastUpdate = privacyPolicy.LastUpdate.ToString();
            var effectiveDate = privacyPolicy.EffectiveDate.ToString();
            var viewModel = new EditPrivacyPolicyViewModel
            {
             //   Id = privacyPolicy.Id,
              //  LastUpdate = privacyPolicy.LastUpdate,
               LastUpdate  = DateUtils.FormatDateTime(lastUpdate, "MMMM, dd yyyy"),
                 EffectiveDate = privacyPolicy.EffectiveDate,                
                Version = privacyPolicy.Version,
                Terms = privacyPolicy.Terms,
                TermsAm = privacyPolicy.TermsAm,
                Policy = privacyPolicy.Policy,
                PolicyAm = privacyPolicy.PolicyAm,
            };
            return viewModel;
        }
    }
}


