

namespace EthioNegarit.WebUI.Controllers
{
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using System.Web.Mvc;
    public class ClosedController : BaseController
    {
        public ClosedController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
          ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, ICacheService cacheService) :
          base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
        }
        // GET: Closed
        public ActionResult Index()
        {
            return View();
        }
    }
}