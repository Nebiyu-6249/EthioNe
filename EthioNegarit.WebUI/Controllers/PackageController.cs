namespace EthioNegarit.WebUI.Controllers
{
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using ViewModels;
    public partial class PackageController : BaseController
    {
    //    private readonly IPaymentService _paymentService;
        private readonly IPackageService _packageService;
      //  private readonly IAdService _adService;
        public PackageController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, IPackageService packageService, IAdService adService, IPaymentService paymentService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, ICacheService cacheService)
            :base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
          //  _paymentService = paymentService;
            _packageService = packageService;
         //   _adService = adService;
        }
        // GET: Package
        public ActionResult Index(Guid? advertId, int month = 1)
        {
			var advert_Id = advertId ?? Guid.Empty;
			using(UnitOfWorkManager.NewUnitOfWork())
            {
                var allPackages = _packageService.GetallPackages();        
                // Localise the package names        
                  foreach (var item in allPackages)
                  {
                      var partialKey = string.Concat("Package.", item.Name);
                      item.DisplayName = LocalizationService.GetResourceString(string.Concat(partialKey, ".Name"));
                      item.Description = LocalizationService.GetResourceString(string.Concat(partialKey, ".Desc"));
                  }
                
                var viewModel = new PackagesListViewModel
                {
                    AdPackages = allPackages.ToList(),
					AdvertId = advert_Id,
                    Month = month
                };
                return View(viewModel);
            }
        }   
    }
}
