namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Core.Constants;
    using Core.Domain.Enums;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Microsoft.Owin.Security;
    using Utilities;
    using ViewModels;
    using Areas.Admin.ViewModels;
    using App_Start;

    /// <summary>
    /// OWIN external (social) login bridge. The provider middleware is configured in
    /// <see cref="Startup"/>. This controller starts the challenge and, on callback, either
    /// logs in an existing member (matched by email) or routes a new one through the existing
    /// registration path (Members/SocialLoginValidator -> MemberRegisterLogic), which creates
    /// and auto-approves the social account and issues the normal forms-auth cookie.
    /// </summary>
    public partial class AuthController : BaseController
    {
        public AuthController(ILogAuditService logAuditService, ILoggingService loggingService,
            IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService,
            ISettingsService settingsService, ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService,
                   localizationService, roleService, settingsService, cacheService)
        {
        }

        // GET: /Auth/ExternalLogin?provider=Google&returnUrl=/
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(provider))
            {
                return RedirectToAction("LogOn", "Members");
            }

            var callbackUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = callbackUrl };
            properties.Dictionary["provider"] = provider;

            // Triggers the OWIN provider middleware (it acts on the 401 + challenge).
            HttpContext.GetOwinContext().Authentication.Challenge(properties, provider);
            return new HttpUnauthorizedResult();
        }

        // GET: /Auth/ExternalLoginCallback (the RedirectUri the provider middleware returns to)
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            var result = await authManager.AuthenticateAsync(Startup.ExternalCookieType);

            if (result == null || result.Identity == null)
            {
                return RedirectWithError("Members.Errors.LogonGeneric");
            }

            var identity = result.Identity;
            var email = FindClaim(identity, ClaimTypes.Email);
            var displayName = FindClaim(identity, ClaimTypes.Name);

            string provider = null;
            if (result.Properties != null && result.Properties.Dictionary != null)
            {
                result.Properties.Dictionary.TryGetValue("provider", out provider);
            }

            // Clear the temporary external cookie now we have the identity.
            authManager.SignOut(Startup.ExternalCookieType);

            if (string.IsNullOrWhiteSpace(email))
            {
                // Providers can withhold email; without it we can't create/link a local account.
                return RedirectWithError("Members.UnableToGetEmailAddress");
            }

            // Existing member with this email? Log them in.
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var existing = MembershipService.GetUserByEmail(email);
                if (existing != null)
                {
                    FormsAuthentication.SetAuthCookie(existing.UserName, true);
                    if (IsSafeReturnUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home", new { area = string.Empty });
                }
            }

            // New user - hand off to the existing registration path (creates + approves + logs in).
            var viewModel = new MemberAddViewModel
            {
                Email = email,
                UserName = string.IsNullOrWhiteSpace(displayName) ? email.Split('@')[0] : displayName,
                Password = StringUtils.RandomString(8),
                LoginType = MapProvider(provider),
                ReturnUrl = returnUrl
            };
            TempData[AppConstants.MemberRegisterViewModel] = viewModel;
            return RedirectToAction("SocialLoginValidator", "Members");
        }

        private static LoginType MapProvider(string provider)
        {
            if (string.Equals(provider, "Google", StringComparison.OrdinalIgnoreCase)) return LoginType.Google;
            if (string.Equals(provider, "Facebook", StringComparison.OrdinalIgnoreCase)) return LoginType.Facebook;
            if (string.Equals(provider, "Microsoft", StringComparison.OrdinalIgnoreCase)) return LoginType.Microsoft;
            return LoginType.Standard;
        }

        private static string FindClaim(ClaimsIdentity identity, string type)
        {
            var claim = identity.FindFirst(type);
            return claim != null ? claim.Value : null;
        }

        private bool IsSafeReturnUrl(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                   && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\");
        }

        private ActionResult RedirectWithError(string resourceKey)
        {
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = LocalizationService.GetResourceString(resourceKey),
                MessageType = GenericMessages.danger
            };
            return RedirectToAction("LogOn", "Members");
        }
    }
}
