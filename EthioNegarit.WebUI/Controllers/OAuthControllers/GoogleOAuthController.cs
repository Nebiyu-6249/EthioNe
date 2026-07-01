using System;
using System.Web.Mvc;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.WebUI.Areas.Admin.ViewModels;
using EthioNegarit.Core.Constants;
using EthioNegarit.WebUI.ViewModels;
using System.Collections.Specialized;
using System.Web.Security;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Utilities;
using Skybrud.Social.Google;
using Skybrud.Social.Google.OAuth;
using EthioNegarit.Core.Interfaces.Services;

namespace EthioNegarit.WebUI.Controllers.OAuthControllers
{
    public partial class GoogleOAuthController : BaseController
    {
        public GoogleOAuthController(ILogAuditService logAuditService, ILoggingService loggingService,
                                    IUnitOfWorkManager unitOfWorkManager,
                                    IMembershipService membershipService,
                                    ILocalizationService localizationService,
                                    IRoleService roleService,
                                    ISettingsService settingsService,
                                    ICacheService cacheService)
            : base(logAuditService, loggingService,
                  unitOfWorkManager,
                  membershipService,
                  localizationService,
                  roleService,
                  settingsService,
                  cacheService)
        {
        }

        //{"web":{"client_id":"159002994469-6atdf04jahq4jnjgi2t1u7opth95i2sf.apps.googleusercontent.com","project_id":"ethionegarit-1751477792164","auth_uri":"https://accounts.google.com/o/oauth2/auth","token_uri":"https://oauth2.googleapis.com/token","auth_provider_x509_cert_url":"https://www.googleapis.com/oauth2/v1/certs","client_secret":"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","redirect_uris":["http://localhost:53986/GoogleLogin"],"javascript_origins":["http://localhost:53986"]}}
        // public string ReturnUrl => string.Concat(SettingsService.GetSettings().ForumUrl.TrimEnd('/'), Url.Action("GoogleLogin"));
        //  public string ReturnUrl => "http://localhost";
        // public string ReturnUrl => string.Concat(SettingsService.GetSettings().ForumUrl, Url.Action("GoogleLogin"));

        //   public string ReturnUrl => SettingsService.GetSettings().ForumUrl;
       public string ReturnUrl => "http://www.ethiopical.com/GoogleLogin";
        public string Callback { get; private set; }
        public string ContentTypeAlias { get; private set; }
        public string PropertyAlias { get; private set; }
        public string Feature { get; private set; }
        /// <summary>
        /// Gets the authorizing code from the query string (if specified).
        /// </summary>
        public string AuthCode => Request.QueryString["code"];
        public string AuthState => Request.QueryString["state"];
        public string AuthErrorReason => Request.QueryString["error_reason"];
        public string AuthError => Request.QueryString["error"];
        public string AuthErrorDescription => Request.QueryString["error_description"];
        public ActionResult GoogleLogin()
        {
            var resultMessage = new GenericMessageViewModel();

            Callback = Request.QueryString["callback"];
            ContentTypeAlias = Request.QueryString["contentTypeAlias"];
            PropertyAlias = Request.QueryString["propertyAlias"];
            Feature = Request.QueryString["feature"];

            if (AuthState != null)
            {
                var stateValue = Session["Ethiopical_" + AuthState] as NameValueCollection;
                if (stateValue != null)
                {
                    Callback = stateValue["Callback"];
                    ContentTypeAlias = stateValue["ContentTypeAlias"];
                    PropertyAlias = stateValue["PropertyAlias"];
                    Feature = stateValue["Feature"];
                }
            }

            if (string.IsNullOrEmpty(SiteConstants.Instance.GooglePlusAppId) ||
                string.IsNullOrEmpty(SiteConstants.Instance.GooglePlusAppSecret))
            {
                resultMessage.Message = "You need to add the Google app credentials";
                resultMessage.MessageType = GenericMessages.danger;
            }
            else
            {
                // //{"web":{"client_id":"159002994469-6atdf04jahq4jnjgi2t1u7opth95i2sf.apps.googleusercontent.com","project_id":"ethionegarit-1751477792164","auth_uri":"https://accounts.google.com/o/oauth2/auth","token_uri":"https://oauth2.googleapis.com/token","auth_provider_x509_cert_url":"https://www.googleapis.com/oauth2/v1/certs","client_secret":"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","redirect_uris":["http://localhost:53986/GoogleLogin"],"javascript_origins":["http://localhost:53986"]}}
                // Configure the OAuth client based on the options of the prevalue options
                var client = new GoogleOAuthClient
                {
                    ClientId = SiteConstants.Instance.GooglePlusAppId,                    
                    ClientSecret = SiteConstants.Instance.GooglePlusAppSecret,
                  RedirectUri = ReturnUrl                
                };

                // Session expired?
                if (AuthState != null && Session["Ethiopical_" + AuthState] == null)
                {
                    resultMessage.Message = "Session Expired";
                    resultMessage.MessageType = GenericMessages.danger;
                }
                // Check whether an error response was received from Google
                if (AuthError != null)
                {
                    resultMessage.Message = AuthErrorDescription;
                    resultMessage.MessageType = GenericMessages.danger;
                    if (AuthState != null) Session.Remove("Ethiopical_" + AuthState);
                }
                // Redirect the user to the Google login dialog
                if (AuthCode == null)
                {
                    // Generate a new unique/random state
                    var state = Guid.NewGuid().ToString();
                    // Save the state in the current user session
                    Session["Ethiopical_" + state] = new NameValueCollection {
                    { "Callback", Callback},
                    { "ContentTypeAlias", ContentTypeAlias},
                    { "PropertyAlias", PropertyAlias},
                    { "Feature", Feature}
                };

                    // Declare the scope
                    var scope = new[] {
                    GoogleScopes.OpenId,
                    GoogleScopes.Email,
                    GoogleScopes.Profile
                };
                    // Construct the authorization URL
                    var url = client.GetAuthorizationUrl(state, scope, GoogleAccessType.Offline, GoogleApprovalPrompt.Force);
                    // Redirect the user
                    return Redirect(url);
                }
                var info = new GoogleAccessTokenResponse();
                try
                {
                    info = client.GetAccessTokenFromAuthorizationCode(AuthCode);
                }
                catch (Exception ex)
                {
                    resultMessage.Message = $"Unable to acquire access token<br/>{ex.Message}";
                    resultMessage.MessageType = GenericMessages.danger;
                }
                try
                {
                    // Initialize the Google service
                    var service = GoogleService.CreateFromRefreshToken(client.ClientIdFull, client.ClientSecret, info.RefreshToken);
                    // Get information about the authenticated user
                    var user = service.GetUserInfo();
                    using (UnitOfWorkManager.NewUnitOfWork())
                    {
                        var userExists = MembershipService.GetUserByEmail(user.Email);
                        if (userExists != null)
                        {
                            // Users already exists, so log them in
                            FormsAuthentication.SetAuthCookie(userExists.UserName, true);
                            resultMessage.Message = LocalizationService.GetResourceString("Members.NowLoggedIn");
                            resultMessage.MessageType = GenericMessages.success;
                            ShowMessage(resultMessage);
                            return RedirectToAction("Index", "Topic");
                        }
                        else
                        {
                            // Not registered already so register them
                            var viewModel = new MemberAddViewModel
                            {
                                Email = user.Email,
                                LoginType = LoginType.Google,
                                Password = StringUtils.RandomString(8),
                                UserName = user.Name,
                                SocialProfileImageUrl = user.Picture,
                                UserAccessToken = info.RefreshToken
                            };
                            // Store the viewModel in TempData - Which we'll use in the register logic
                            TempData[AppConstants.MemberRegisterViewModel] = viewModel;
                            return RedirectToAction("SocialLoginValidator", "Members");
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultMessage.Message = $"Unable to get user information<br/>{ex.Message}";
                    resultMessage.MessageType = GenericMessages.danger;
                    LoggingService.Error(ex);
                }
            }
            ShowMessage(resultMessage);
            return RedirectToAction("LogOn", "Members");
        }
    }
}