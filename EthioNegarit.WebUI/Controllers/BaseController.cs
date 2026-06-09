using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.WebUI.Areas.Admin.ViewModels;
using MembershipUser = EthioNegarit.Core.Domain.Entities.MembershipUser;
namespace EthioNegarit.WebUI.Controllers
    {
    using Utilities;

    /// <summary>
    /// A base class for the white site controllers
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly IUnitOfWorkManager UnitOfWorkManager;
        protected readonly IMembershipService MembershipService;
        protected readonly ILocalizationService LocalizationService;
        protected readonly IRoleService RoleService;
        protected readonly ISettingsService SettingsService;
        protected readonly ILoggingService LoggingService;
        protected readonly ICacheService CacheService;
        protected readonly ILogAuditService LogAuditService;      
        protected MembershipUser LoggedOnReadOnlyUser;
        protected MembershipRole UsersRole;
        public BaseController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, ICacheService cacheService)
        {
            UnitOfWorkManager = unitOfWorkManager;
            MembershipService = membershipService;
            LocalizationService = localizationService;
            RoleService = roleService;
            SettingsService = settingsService;
            CacheService = cacheService;
            LoggingService = loggingService;
            LogAuditService = logAuditService;
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                LoggedOnReadOnlyUser = UserIsAuthenticated ? MembershipService.GetUser(Username, true) : null;
                UsersRole = LoggedOnReadOnlyUser == null ? RoleService.GetRole(AppConstants.GuestRoleName, true) : LoggedOnReadOnlyUser.Roles.FirstOrDefault();
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {   /*      
            //Stores the Request in an Accessible object
            var request = filterContext.HttpContext.Request;
 
            //Generate an audit
            Audit audit = new Audit()
            {
                AuditID = Guid.NewGuid(),
                IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
                URLAccessed = request.RawUrl,
                Platform = request.UserAgent.ToString(),
                TimeAccessed = DateTime.UtcNow,
                UserName = (request.IsAuthenticated) ? System.Web.HttpContext.Current.User.Identity.Name : "Anonymous",
               
                //  UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
                //  Browser = System.Web.HttpContext.Current.Request.Browser.ToString(),
                // Browser = request.Browser.ToString(),            
                //    Browser = request.UserLanguages.ToString(),               
                //  Header = request.Headers.ToString(),
                //  Header = request.UserHostName.ToString(),
            };
            
          //  LogAuditService.Add(audit);
         //   base.OnActionExecuting(filterContext);
         */
          ////////////////////////////////////////////////////////////
            var controller = filterContext.RouteData.Values["controller"];
            var action = filterContext.RouteData.Values["action"];
            var area = filterContext.RouteData.DataTokens["area"] ?? string.Empty;
            var settings = SettingsService.GetSettings();

            // Check if forum is closed
            if (settings.IsClosed && !filterContext.IsChildAction)
            {
                // Only redirect if its closed and user is NOT in the admin
                if (controller.ToString().ToLower() != "closed" && controller.ToString().ToLower() != "members" && !area.ToString().ToLower().Contains("admin"))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Closed" }, { "action", "Index" } });
                }
            }

            // Check if they need to agree to permissions
            if (SettingsService.GetSettings().AgreeToTermsAndConditions == true && !filterContext.IsChildAction && LoggedOnReadOnlyUser != null && LoggedOnReadOnlyUser.HasAgreedToTermsAndConditions != true)       
            {
                // Only redirect if its closed and user is NOT in the admin
                if ((action.ToString().ToLower() != "agreement" && action.ToString().ToLower() != "terms") && !area.ToString().ToLower().Contains("admin"))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Privacy" }, { "action", "Agreement" } });
                }
            }
            // If the forum is new members need approving and the user is not approved, log them out
            if (LoggedOnReadOnlyUser != null && !LoggedOnReadOnlyUser.IsApproved && settings.NewMemberEmailConfirmation == true)
            {
                FormsAuthentication.SignOut();
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("Members.MemberEmailAuthorisationNeeded"),
                    MessageType = GenericMessages.success
                };
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
            }

            // If the user is banned - Log them out.
            if (LoggedOnReadOnlyUser != null && LoggedOnReadOnlyUser.IsBanned)
            {
                FormsAuthentication.SignOut();
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("Members.NowBanned"),
                    MessageType = GenericMessages.danger
                };
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
            }
        }
        protected bool UserIsAuthenticated => System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        protected bool UserIsAdmin => User.IsInRole(AppConstants.AdminRoleName);
        protected string Domain => CacheService.CachePerRequest(CacheKeys.Domain, StringUtils.ReturnCurrentDomain);
        protected void ShowMessage(GenericMessageViewModel messageViewModel)
        {         
            TempData[AppConstants.MessageViewBagName] = messageViewModel;
        }
        protected string Username => UserIsAuthenticated ? System.Web.HttpContext.Current.User.Identity.Name : null;
        internal ActionResult ErrorToHomePage(string errorMessage)
        {
            // Use temp data as its a redirect
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = errorMessage,
                MessageType = GenericMessages.danger
            };
            // Not allowed in here so
            return RedirectToAction("Index", "Home");
        }
    }
    public class UserNotLoggedOnException : System.Exception
    {
    }
}