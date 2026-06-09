namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Web.Mvc;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Areas.Admin.ViewModels;
    using ViewModels;
    using Core.Domain.Entities;

    public partial class LocationController : BaseController
    {
        private readonly IAdService _adService;
        private readonly IPackageService _packageService;
        private readonly IAdvertFileService _advertFileService;
        private readonly IRoleService _roleService;
        private readonly IMembershipUserPointsService _membershipUserPointsService;

        private readonly ILocationService _locationService;
        public LocationController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
      ILocalizationService localizationService, IRoleService roleService, IAdService adService, IAdvertFileService advertFileService,
   ISettingsService settingsService, IPackageService packageService, ILocationService locationService, ICacheService cacheService, IMembershipUserPointsService membershipUserPointsService)
                : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
                {
            _adService = adService;
            _packageService = packageService;
            _advertFileService = advertFileService;
            _roleService = roleService;
            _membershipUserPointsService = membershipUserPointsService;
            _locationService = locationService;
        }
        // GET: Location
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get(Guid advertId)
        {
            if (Request.IsAjaxRequest())
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    var advert = _adService.Get(advertId);
                    var locations = _locationService.GetLocations(advertId);
                    var viewModel = new LocationViewModel
                    {
                        AdvertId = advertId,
                        Advertisement = advert,
                        Locations = locations
                    };
                    return PartialView(viewModel);
                }

            }
            return null;
        }

        [HttpPost]
        [Authorize]
        // [ValidateAntiForgeryToken]
        public ActionResult Add(LocationViewModel viewModel)
        {
            Location location;
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var advert = _adService.Get(viewModel.AdvertId);
                    location = new Location
                    {
                        Advertisement = advert,
                        Latitude = viewModel.Latitude,
                        Longitude = viewModel.Longitude,
                        CityName = viewModel.CityName,
                        Description = viewModel.Description
                    };
                    location = _locationService.Add(location);
                    try
                    {
                        unitOfWork.Commit();
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Location Successfully Submitted"),
                            MessageType = GenericMessages.success
                        });
                        return RedirectToAction("MyAds", "Ad");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Submition Failed"),
                            MessageType = GenericMessages.info
                        });
                    }
                    return RedirectToAction("MyAds", "Ad");
                }
            }
            return null;
        }    
        public ActionResult View(Guid advertId)
        {
            // Get the ad
            var advert = _adService.Get(advertId);
            var locations = _locationService.GetLocations(advertId);
            // Example: Get location data from a database or other source
            var viewModel = new LocationViewModel
            {
                Locations = locations,
            };
            return PartialView(viewModel);
        }
        [HttpPost]
        public void Delete(Guid advertId)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                // Get Ad
                var location = _locationService.Get(advertId);
                //  var advert = _adService.Get(advertId);
                //    var advert = location.Advertisement;
                //  if (advert.User.Id == LoggedOnReadOnlyUser.Id || User.IsInRole(AppConstants.AdminRoleName))
                // {
                _locationService.Delete(location);
                    unitOfWork.SaveChanges();
                    try
                    {
                        unitOfWork.Commit();
                      /*  TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Post.Deleted"),
                            MessageType = GenericMessages.success
                        };*/
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
               // }
            }
        }

        /*
         public static string GetUsersIpAddress()
        {
            var context = HttpContext.Current;
            var serverName = context.Request.ServerVariables["SERVER_NAME"];
            if (serverName.ToLower().Contains("localhost"))
            {
                return serverName;
            }
            var ipList = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return !string.IsNullOrEmpty(ipList) ? ipList.Split(',')[0] : context.Request.ServerVariables["REMOTE_ADDR"];
        }
        */
    }
}