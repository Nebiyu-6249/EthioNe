using System;
using System.Linq;
using System.Web.Mvc;
using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.WebUI.Areas.Admin.ViewModels;
using System.Collections.Generic;

namespace EthioNegarit.WebUI.Areas.Admin.Controllers
{
    public class AdminHelpController : BaseAdminController
    {
        private readonly IHelpService _helpService;

        public AdminHelpController(ILoggingService loggingService,
                                        IUnitOfWorkManager unitOfWorkManager,
                                       IMembershipService membershipService,
                                       ILocalizationService localizationService,
                                       IHelpService helpService,
                                       ISettingsService settingsService)
            : base(loggingService, unitOfWorkManager, membershipService, localizationService, settingsService)
        {
            _helpService = helpService;
        }
        public ActionResult Manage(int? p, string search)
        {
            return RedirectToAction("Index", new { p, search });
        }
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public PartialViewResult GetFAQs()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var viewModel = new ListFAQViewModel()
                {
                    Faqs = _helpService.GetFAQs()
                };
                return PartialView(viewModel);
            }
        }
        public List<SelectListItem> LanguageSelection()
        {
            var alllanguages = LocalizationService.AllLanguages;
            var languages = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            foreach (var language in alllanguages)
            {
              languages.Add(new SelectListItem { Text = language.LanguageCulture, Value = language.Name });
            }
            return languages;
        }
        public ActionResult CreateFAQ()
        {
                var viewModel = new CreateEditFAQViewModel
                {
                    Languages = new SelectList(LanguageSelection(), "Text", "Value"),
                 //   Month = 1 // Optionally, set a default selected value				
                };
                return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFAQ(CreateEditFAQViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var faq = new FAQ
                        {
                            Question = viewModel.Question,
                            Replay = viewModel.Replay,
                         Language = viewModel.Language.ToString()
                        };
                        _helpService.Add(faq);
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "FAQ Created Successfully",
                            MessageType = GenericMessages.success
                        };
                        unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        unitOfWork.Rollback();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "There was an error creating FAQ");
            }
            return RedirectToAction("Index");
        }
      public ActionResult Edit(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var faq = _helpService.GetFAQ(id);
                var viewModel = new CreateEditFAQViewModel
                {
                    Id = faq.Id,
                    Language = faq.Language,
                    Question = faq.Question,
                    Replay = faq.Replay,
                    Languages = new SelectList(LanguageSelection(), "Text", "Value")			
                };
                return View(viewModel);
            }
        }
       
        [HttpPost]
        public ActionResult Edit(CreateEditFAQViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var faq = _helpService.GetFAQ(viewModel.Id);
                        faq.Question = viewModel.Question;
                        faq.Replay = viewModel.Replay;
                        //faq.HelpCategory = helpViewModel.HelpCategory;   
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "FAQ Updated Successfully",
                            MessageType = GenericMessages.success
                        };
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "FAQ Update Failed",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
            }
            return View(viewModel);
        }
        public ActionResult DeleteFAQ(Guid id)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var faq = _helpService.GetFAQ(id);
                    _helpService.Delete(faq);
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "FAQ Deleted",
                        MessageType = GenericMessages.success
                    };
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                }
                return RedirectToAction("Index");
            }
        }


        /*
                public ActionResult DeleteFAQConfirmation(Guid id)
                {
                    using (UnitOfWorkManager.NewUnitOfWork())
                    {
                        var help = _helpService.Get(id);
                        //  var subCats = _categoryService.GetAllSubCategories(id).ToList();
                        var viewModel = new DeleteSupportViewModel
                        {
                            Id = help.Id,
                            Help = help,
                            //   SubCategories = subCats
                        };

                        return View(viewModel);
                    }
                }
                */ 
    }
}