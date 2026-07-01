namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Core.Constants;
    using Areas.Admin.ViewModels;
    using ViewModels;
    using Core.Domain.Entities;
    using System.Collections.Generic;
    using System.Web.Hosting;
    using System.IO;
    using Application;
    using Core.Domain.General;
    using Utilities;
    using Core.Domain.Enums;
    using System.Data.SqlTypes;
    using System.Text;
    public partial class AdController : BaseController
    {
        private readonly IAdService _adService;
        private readonly IPackageService _packageService;
        private readonly IPaymentService _paymentService;
        private readonly IAdvertFileService _advertFileService;
        private readonly IRoleService _roleService;
        private readonly IMembershipUserPointsService _membershipUserPointsService;
        private readonly IBannedWordService _bannedWordService;
        private readonly ILocationService _locationService;
        private readonly IEmailService _emailService;
        public AdController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
           ILocalizationService localizationService, IRoleService roleService, IAdService adService, IAdvertFileService advertFileService,
        ISettingsService settingsService, IBannedWordService bannedWordService, IPackageService packageService, IPaymentService paymentService, ILocationService locationService, ICacheService cacheService, IMembershipUserPointsService membershipUserPointsService, IEmailService emailService)
                : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _adService = adService;
            _advertFileService = advertFileService;
            _packageService = packageService;
            _paymentService = paymentService;
            _roleService = roleService;
            _membershipUserPointsService = membershipUserPointsService;
            _bannedWordService = bannedWordService;
            _locationService = locationService;
            _emailService = emailService;
        }
        // GET: Advertisement
        public ActionResult Index(int? p)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                var canCreateAd = false;
                var trialAdEnabled = false;
                if (LoggedOnReadOnlyUser != null)
                {
                    var memberAds = LoggedOnReadOnlyUser.Advertisements;
                    var pendingAds = memberAds.Where(x => x.AdStatus == AdStatus.Partial || x.AdStatus == AdStatus.Pending);
                    if (!pendingAds.Any() && settings.EnableAd)
                    {
                        canCreateAd = true;
                    }
                    if (LoggedOnReadOnlyUser.EnableAdtrial != false)
                    {
                        if (!memberAds.Any() && settings.EnableAd && settings.AdTrialDays > 0)
                        {
                            trialAdEnabled = true;
                        }
                    }
                }
                // Set the page index
                var pageIndex = p ?? 1;
                // Get the adverts             
                var adverts = _adService.GetActiveAds(pageIndex, settings.TopicsPerPage, SiteConstants.Instance.AdListSize);
                // create the view model
                var viewModel = new AdListViewModel
                {
                    Advertisements = adverts,
                    UserCanCreatAd = canCreateAd,
                    TrialAdEnabled = trialAdEnabled,
                    PageIndex = pageIndex,
                    TotalCount = adverts.TotalCount,
                    TotalPages = adverts.TotalPages
                };
                return View(viewModel);
            }
        }
        [Authorize(Roles = AppConstants.AdminRoleName)]
        public ActionResult List(int? p)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Set the page index
                var pageIndex = p ?? 1;
                var settings = SettingsService.GetSettings();
                var viewQuerystring = Request.QueryString[AppConstants.AdViewQuery];
                var query = !string.IsNullOrEmpty(viewQuerystring) ? EnumUtils.ReturnEnumValueFromString<AdViewQuery>(viewQuerystring) : AdViewQuery.All;
                // Get the ads             
                var adverts = _adService.GetAllAds(pageIndex, settings.TopicsPerPage, SiteConstants.Instance.AdminListPageSize, query);
                // create the view model
                var viewModel = new AdListViewModel
                {
                    Advertisements = adverts,
                    PageIndex = pageIndex,
                    TotalCount = adverts.TotalCount,
                    TotalPages = adverts.TotalPages
                };
                return View(viewModel);
            }
        }
        [Authorize]
        public ActionResult MyAds(int? p)
        {
            var pageIndex = p ?? 1;
            var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                // Get the Ads
                var adverts = _adService.GetMemberAds(pageIndex, SiteConstants.Instance.PagingGroupSize, loggedOnUser);
                // create the view model
                var viewModel = new AdListViewModel
                {
                    Advertisements = adverts,
                    PageIndex = pageIndex,
                    TotalCount = adverts.TotalCount,
                    TotalPages = adverts.TotalPages
                };
                return View(viewModel);
            }
        }
        public PartialViewResult CreateAdButton()
        {
            var settings = SettingsService.GetSettings();
            var canCreateAd = false;
            var viewModel = new CreateAdButtonViewModel();
            if (LoggedOnReadOnlyUser != null)
            {
                var memberAds = LoggedOnReadOnlyUser.Advertisements;
                var pendingAds = memberAds.Where(x => x.AdStatus == AdStatus.Partial || x.AdStatus == AdStatus.Pending);
                if (!pendingAds.Any() && settings.EnableAd)
                {
                    canCreateAd = true;
                }
                viewModel.UserCanCreatAd = canCreateAd;
            }
            return PartialView(viewModel);
        }
       
        public ActionResult Create(bool trial = false)
        {
            var settings = SettingsService.GetSettings();
            //    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);			
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                if (settings.EnableAd)
                {
                    var viewModel = new CreateEditAdViewModel
                    {
                        //IsTrial = trial,
                        Months = new SelectList(MonthSelection(), "Value", "Name"),
                        Month = 1 // Optionally, set a default selected value				
                    };
                    if (trial == true && settings.AdTrialDays > 0)
                    {
                        viewModel.IsTrial = true;
                    }
                    else
                    {
                        viewModel.IsTrial = false;
                    }
                    return View(viewModel);
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
            }
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateEditAdViewModel viewModel)
        {
            var settings = SettingsService.GetSettings();
            Advertisement advert;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                // We get the banned words here 
                var bannedWordsList = _bannedWordService.GetAll();
                List<string> bannedWords = null;
                if (bannedWordsList.Any())
                {
                    bannedWords = bannedWordsList.Select(x => x.Word).ToList();
                }
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                // Create the Advertisement model
                advert = new Advertisement();
                advert.User = loggedOnUser;
                advert.Name = viewModel.Name;
                advert.MotoSlogan = viewModel.Moto;
                advert.Address = viewModel.Address;
                advert.Email = viewModel.Email;
                advert.Line = viewModel.Line;
                advert.LinkText = viewModel.LinkText;
                advert.DateLastActive = (DateTime)SqlDateTime.MinValue;
                advert.DateCreated = DateTime.UtcNow;
                advert.DateUpdate = DateTime.UtcNow;
                advert.PostCount = 0;
                advert.EnableReminder = viewModel.EnableReminder;
                advert.Month = (int?)viewModel.Month ?? 0;
                advert.About = _bannedWordService.SanitiseBannedWords(viewModel.About, bannedWords);
                if (viewModel.Link != null)
                {
                    advert.LinkUrl = viewModel.Link;
                }
                if (viewModel.IsTrial && settings.AdTrialDays > 0)
                {
                    var package = _packageService.GetBasicPackage();
                    advert.AdStatus = AdStatus.Active;
                    advert.Package = package;
                    advert.DateStart = DateTime.UtcNow;
                    advert.DateEnd = DateTime.UtcNow.AddDays(SettingsService.GetSettings().AdTrialDays);
                    loggedOnUser.EnableAdtrial = false;
                }
                else
                {
                    advert.DateStart = (DateTime)SqlDateTime.MinValue;
                    advert.DateEnd = (DateTime)SqlDateTime.MinValue;
                    advert.AdStatus = AdStatus.Partial;
                }
                var country = StringUtils.GetUserCountryByIp();
                if (string.IsNullOrEmpty(country))
                {
                    advert.Country = AppConstants.DefaultCountry;
                }
                else
                {
                    advert.Country = country;
                }
                /**************************************/
                // Create the Advertisement
                advert = _adService.Add(advert);
                unitOfWork.SaveChanges();

                /**********Files*************/
                // store the file inside 
                var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, advert.Id));
                if (!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                if (viewModel.Image != null && viewModel.Image.ContentLength > 0)
                {
                    var image = viewModel.Image;
                    var uploadResult = AppHelpers.UploadFile(image, uploadFolderPath, LocalizationService, true);

                    if (!uploadResult.UploadSuccessful)
                    {
                        unitOfWork.Rollback();
                        return View(viewModel);
                    }
                    advert.Image = uploadResult.UploadedFileName;
                    unitOfWork.SaveChanges();
                }
                try
                {
                    unitOfWork.Commit();
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("Ad.Created"),
                        MessageType = GenericMessages.success
                    };
                    if (viewModel.IsTrial)
                    {
                        return RedirectToAction("MyAds", "Ad");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Package", new { advertId = advert.Id, month = viewModel.Month, area = string.Empty });
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
                return RedirectToAction("Index", "Package", new { advertId = advert.Id, area = string.Empty });
            }
        }
        [Authorize]
        public ActionResult Edit(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Get the ad
                var advert = _adService.Get(id);
                if (advert != null)
                {
                    if (advert.User.Id == LoggedOnReadOnlyUser.Id || User.IsInRole(AppConstants.AdminRoleName))
                    {
                        var viewModel = new CreateEditAdViewModel
                        {
                            Id = advert.Id,
                            Name = advert.Name,
                            Link = advert.LinkUrl,
                            About = advert.About,
                            Moto = advert.MotoSlogan,
                            Email = advert.Email,
                            Line = advert.Line,
                            Address = advert.Address,
                            LinkText = advert.LinkText,
                            EnableReminder = advert.EnableReminder,
                            AdStatus = advert.AdStatus,
                            // LogoImg = advert.Logo,
                            Img = advert.Image,
                            Months = new SelectList(MonthSelection(), "Value", "Name"),
                        };
                        return View(viewModel);
                    }
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateEditAdViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var advert = _adService.Get(viewModel.Id);
                // We get the banned words here 
                var bannedWordsList = _bannedWordService.GetAll();
                List<string> bannedWords = null;
                if (bannedWordsList.Any())
                {
                    bannedWords = bannedWordsList.Select(x => x.Word).ToList();
                }
                if (ModelState.IsValid)
                {
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    if (advert.User.Id == LoggedOnReadOnlyUser.Id || User.IsInRole(AppConstants.AdminRoleName))
                    {
                        advert.About = _bannedWordService.SanitiseBannedWords(viewModel.About, bannedWords);
                        if (viewModel.Name != null)
                        {
                            advert.Name = _bannedWordService.SanitiseBannedWords(viewModel.Name, bannedWords);
                        }
                        else
                        {
                            advert.Name = StringUtils.ReturnAmountWordsFromString(viewModel.About, 10);
                        }

                        if (viewModel.Moto != null)
                        {
                            advert.MotoSlogan = _bannedWordService.SanitiseBannedWords(viewModel.Moto, bannedWords);
                        }
                        if (viewModel.Link != null)
                        {
                            advert.LinkUrl = viewModel.Link;
                        }

                        advert.Address = viewModel.Address;
                        advert.Email = viewModel.Email;
                        advert.Line = viewModel.Line;
                        advert.LinkText = viewModel.LinkText;
                        advert.DateUpdate = DateTime.UtcNow;
                        advert.EnableReminder = viewModel.EnableReminder;

                        /**********Image*************/
                        // store the file inside                       
                        var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, advert.Id));
                        if (!Directory.Exists(uploadFolderPath))
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }
                        if (viewModel.Image != null && viewModel.Image.ContentLength > 0)
                        {
                            var image = viewModel.Image;
                            var uploadResult = AppHelpers.UploadFile(image, uploadFolderPath, LocalizationService, true);

                            if (!uploadResult.UploadSuccessful)
                            {
                                unitOfWork.Rollback();
                                return View(viewModel);
                            }
                            advert.Image = uploadResult.UploadedFileName;
                            unitOfWork.SaveChanges();
                        }
                        try
                        {
                            unitOfWork.Commit();
                            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Ad.Updated"),
                                MessageType = GenericMessages.info
                            };
                            return RedirectToAction("MyAds", "Ad");
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }
                    }
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
                }
            }
            return View(viewModel);
        }
        [HttpPost]
        [Authorize]
        public void Delete(Guid id)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                // Get Ad
                var advert = _adService.Get(id);
                if (advert.User.Id == LoggedOnReadOnlyUser.Id || User.IsInRole(AppConstants.AdminRoleName))
                {
                    var advertFiles = new List<AdvertisementFile>();
                    advertFiles.AddRange(advert.Files);
                    foreach (var advertFile in advertFiles)
                    {
                        _advertFileService.Delete(advertFile);
                    }
                    advert.Files.Clear();
                    _adService.Delete(advert);
                    unitOfWork.SaveChanges();
                    try
                    {
                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Post.Deleted"),
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.GenericMessage"),
                            MessageType = GenericMessages.danger
                        });

                    }
                }
            }
        }
        public ActionResult Show(string slug)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var advert = _adService.GetAdvert(slug);
                // var locations = _locationService.GetLocations(advert.Id);
                if (advert != null)
                {
                    var viewModel = new AdViewModel
                    {
                        Advertisement = advert,
                        hasMap = _locationService.GetLocations(advert.Id).Any(),
                        hasVideo = _adService.GetVideos(advert.Id).Any()
                    };

                    /* var videoFile = _adService.GetVideo(advert.Id);
                     if (videoFile != null)
                     {
                         viewModel.hasVideo = true;
                     }*/
                    var updateDatabase = false;
                    var addView = !(UserIsAuthenticated && LoggedOnReadOnlyUser.Id == advert.User.Id);
                    if (addView)
                    {
                        updateDatabase = true;
                    }
                    if (!BotUtils.UserIsBot() && updateDatabase)
                    {
                        if (addView)
                        {
                            // Increase the advert views
                            // advert.Views = (advert.Views + 1);
                        }
                        try
                        {
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            LoggingService.Error(ex);
                        }
                    }
                    return View(viewModel);
                }
            }

            // If we get here the user has no permission to try and edit the post
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }
        public PartialViewResult Top()
        {
            // var timespan = DateTime.UtcNow.AddHours(-1);
            //  var timespan = DateTime.UtcNow.AddMinutes(-20);
            // Only render ads when advertising is enabled site-wide.
            if (SettingsService.GetSettings().EnableAd && Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var country = StringUtils.GetUserCountryByIp();
                    var activeAds = string.IsNullOrEmpty(country) ? _adService.GetActiveAds() : _adService.GetActiveAds(country);
                    //    var advert = activeAds.FirstOrDefault(x => x.DateLastActive <= timespan || x.DateLastActive == null);
                    var advert = activeAds.FirstOrDefault();
                    if (advert != null)
                    {
                        UpdateAd(advert);
                        var viewModel = new AdViewModel
                        {
                            Advertisement = advert
                        };
                        try
                        {
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }
                        return PartialView(viewModel);
                    }
                }
            }
            return null;
        }
        public PartialViewResult Bottom()
        {
            //   var timespan = DateTime.UtcNow.AddMinutes(-15);
            // Only render ads when advertising is enabled site-wide.
            if (SettingsService.GetSettings().EnableAd && Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var country = StringUtils.GetUserCountryByIp();
                    var activeAds = string.IsNullOrEmpty(country) ? _adService.GetActiveAds() : _adService.GetActiveAds(country);
                    // var advert = activeAds.FirstOrDefault(x => x.DateLastActive <= timespan || x.DateLastActive == null);
                    var advert = activeAds.FirstOrDefault();
                    if (advert != null)
                    {
                        UpdateAd(advert);
                        var viewModel = new AdViewModel
                        {
                            Advertisement = advert
                        };
                        try
                        {
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            //   ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }
                        return PartialView(viewModel);
                    }
                }
            }
            return null;
        }
        public PartialViewResult BottomLeft()
        {
            var timespan = DateTime.UtcNow.AddMinutes(-10);
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var country = StringUtils.GetUserCountryByIp();
                    var activeAds = string.IsNullOrEmpty(country) ? _adService.GetActiveAds() : _adService.GetActiveAds(country);
                    //    var advert = activeAds.FirstOrDefault(x => x.DateLastActive <= timespan || x.DateLastActive == null);
                    var advert = activeAds.FirstOrDefault(x => x.DateLastActive <= timespan);
                    if (advert != null)
                    {
                        UpdateAd(advert);
                        var viewModel = new AdViewModel
                        {
                            Advertisement = advert,
                            hasVideo = _adService.GetVideos(advert.Id).Any()
                        };
                        try
                        {
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }
                        return PartialView(viewModel);
                    }
                }
            }
            return null;
        }
        public PartialViewResult BottomRight()
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var country = StringUtils.GetUserCountryByIp();
                    var activeAds = string.IsNullOrEmpty(country) ? _adService.GetActiveAds() : _adService.GetActiveAds(country);
                    var advert = activeAds.FirstOrDefault();
                    if (advert != null)
                    {
                        UpdateAd(advert);
                        var viewModel = new AdViewModel
                        {
                            Advertisement = advert,
                            hasVideo = _adService.GetVideos(advert.Id).Any()
                        };
                        try
                        {
                            //  UpdateAd(advert);
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }
                        return PartialView(viewModel);
                    }
                }
            }
            return null;
        }
        public PartialViewResult Slide()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var country = StringUtils.GetUserCountryByIp();
                var activeAds = string.IsNullOrEmpty(country) ? _adService.GetActiveAds() : _adService.GetActiveAds(country).ToList();
                // create the view model
                var viewModel = new AdListViewModel
                {
                    Advertisements = activeAds,
                };
                return PartialView(viewModel);
            }
        }
        public PartialViewResult AutoSlide()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var country = StringUtils.GetUserCountryByIp();
                var activeAds = string.IsNullOrEmpty(country) ? _adService.GetActiveAds() : _adService.GetActiveAds(country).ToList();
                // create the view model
                var viewModel = new AdListViewModel
                {
                    Advertisements = activeAds,
                };
                return PartialView(viewModel);
            }
        }
        public List<object> MonthSelection()
        {
            var months = new List<object>
            {
        new { Value = 1, Name = LocalizationService.GetResourceString("Ad.Label.Month") },
        new { Value = 3, Name = LocalizationService.GetResourceString("Ad.Label.Quarter")},
        new { Value = 6, Name = LocalizationService.GetResourceString("Ad.Label.SemiAnnual")},
         new { Value = 12, Name = LocalizationService.GetResourceString("Ad.Label.Annual")}
            };
            return months;
        }
        private void UpdateAd(Advertisement ad)
        {
            ad.DateLastActive = DateTime.UtcNow;
            ad.PostCount = (ad.PostCount + 1);
            if (ad.DateEnd <= DateTime.UtcNow)
            {
                ad.AdStatus = AdStatus.Completed;
            }
            if (ad.DateEnd >= DateTime.UtcNow.AddDays(-AppConstants.AdReminderDaysRemains) && ad.EnableReminder == true)
            {
                if (ad.ReminderLastSent == null || ad.ReminderLastSent >= DateTime.UtcNow.AddDays(AppConstants.AdReminderDaysRemains))
                {
                    ad.ReminderLastSent = DateTime.UtcNow;
                    //   var email = new Email();
                    var settings = SettingsService.GetSettings();
                    var forumBase = settings.ForumUrl.TrimEnd('/');
                    var actionPath = "Ad/MyAds";
                    var url = new Uri($"{forumBase}{actionPath}");
                    var sb = new StringBuilder();
                    var subject = "Notification of remaining days of advertising/የማስታወቂያ ቀሪ ቀናት ማሳወቂያ";
                    //Send Reminder Message
                    sb.AppendFormat("<p>{0}</p>", string.Format("At EthioNegarit As you requested, we will notify you of the expiration date of your ad. There are only a few days left until your ad expires.If you would like to extend your ad period or have any other questions about your ad, please follow the link below.", settings.ForumName));
                    sb.AppendFormat("<p>{0}</p>", string.Format("በ EthioNegarit. የማስታወቂያ ማብቂያ ጊዜ እንድናሳውቅዎ በጠየቁት መሰረት የማስታወቂያዎ ጊዜ ሊያበቃ የቀሩት ጥቂት ቀናት ናችው ፡፡ የማስታወቂያ ጊዜዎን ለማራዘም ወይም ሌላ ስለ ማስታወቂያዎ የሚፈልጉት ነገር ካለ እባክዎ ከታች ያለውን ሊንክ ይከተሉ።", settings.ForumName));
                    sb.AppendFormat("<p>{0}</p>", string.Format("As you requested", settings.ForumName));
                    sb.AppendFormat("<p><a href=\"{0}\">{0}</a></p>", url);
                    var email = new Email
                    {
                        EmailTo = ad.User.Email,
                        NameTo = ad.User.UserName,
                        Subject = subject
                    };
                    sb.AppendFormat("<p><a href=\"{0}\">{0}</a></p>", url);
                    // sb.AppendFormat("<p><a href=\"{0}\">{0}</a></p>", string.Concat(Domain, ad.NiceUrl));
                    email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                    _emailService.SendMail(email);
                }
            }
        }
        public PartialViewResult Reminder()
        {
            if (Request.IsAjaxRequest())
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    if (UserIsAuthenticated)
                    {
                        //Get Member
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                        var memberAds = loggedOnUser.Advertisements.Where(x => x.AdStatus == AdStatus.Active && x.DateEnd >= DateTime.UtcNow.AddDays(-AppConstants.AdReminderDaysRemains)).ToList();
                        if (memberAds.Any())
                        {
                            var viewModel = new MemberNotificationViewModel
                            {
                                Subject = LocalizationService.GetResourceString("Ad.Notification.Subject"),
                                Notice = string.Format(LocalizationService.GetResourceString("Ad.Notification.Message"), loggedOnUser.UserName)
                            };
                            return PartialView(viewModel);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
    }
}