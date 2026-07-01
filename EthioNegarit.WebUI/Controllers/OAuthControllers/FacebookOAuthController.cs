using System;
using System.Web.Mvc;
using System.Web.Security;
using Skybrud.Social.Facebook;
using Skybrud.Social.Facebook.OAuth;
using Skybrud.Social.Facebook.Options.User;
using EthioNegarit.Utilities;
using EthioNegarit.Core.Constants;
using EthioNegarit.WebUI.ViewModels;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Interfaces.Services;

using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.WebUI.Areas.Admin.ViewModels;

namespace EthioNegarit.WebUI.Controllers.OAuthControllers
{
    public partial class FacebookOAuthController : BaseController
    {
        public FacebookOAuthController(ILogAuditService logAuditService, ILoggingService loggingService,
                                        IUnitOfWorkManager unitOfWorkManager,
                                        IMembershipService membershipService,
                                        ILocalizationService localizationService,
                                        IRoleService roleService,
                                        ISettingsService settingsService,
                                        ICacheService cacheService) : base(logAuditService, loggingService,
                                                                                unitOfWorkManager,
                                                                                membershipService,
                                                                                localizationService,
                                                                                roleService,
                                                                                settingsService,
                                                                                cacheService)
        {

        }

        public string ReturnUrl
        {
            get
            {
                 return string.Concat(SettingsService.GetSettings().ForumUrl.TrimEnd('/'), Url.Action("FacebookLogin"));
              //  return "http://www.ethiopical.com/FacebookLogin";
                  //    return "http://www.ethiopical.com";
            }
        }

        public string Callback { get; private set; }

        public string ContentTypeAlias { get; private set; }

        public string PropertyAlias { get; private set; }

        /// <summary>
        /// Gets the authorizing code from the query string (if specified).
        /// </summary>
        public string AuthCode => Request.QueryString["code"];

        public string AuthState => Request.QueryString["state"];

        public string AuthErrorReason => Request.QueryString["error_reason"];

        public string AuthError => Request.QueryString["error"];

        public string AuthErrorDescription => Request.QueryString["error_description"];

        public ActionResult FacebookLogin()
        {
            var resultMessage = new GenericMessageViewModel();

            Callback = Request.QueryString["callback"];
            ContentTypeAlias = Request.QueryString["contentTypeAlias"];
            PropertyAlias = Request.QueryString["propertyAlias"];

            if (AuthState != null)
            {
                var stateValue = Session["Ethiopical_" + AuthState] as string[];
                if (stateValue != null && stateValue.Length == 3)
                {
                    Callback = stateValue[0];
                    ContentTypeAlias = stateValue[1];
                    PropertyAlias = stateValue[2];
                }
            }

            // Get the prevalue options
            if (string.IsNullOrEmpty(SiteConstants.Instance.FacebookAppId) ||
                string.IsNullOrEmpty(SiteConstants.Instance.FacebookAppSecret))
            {
                resultMessage.Message = "You need to add the Facebook app credentials";
                resultMessage.MessageType = GenericMessages.danger;
            }
            else
            {

                // Settings valid move on
                // Configure the OAuth client based on the options of the prevalue options
                var client = new FacebookOAuthClient
                {
                    AppId = SiteConstants.Instance.FacebookAppId,
                    AppSecret = SiteConstants.Instance.FacebookAppSecret,
                    RedirectUri = ReturnUrl
                };

                // Session expired?
                if (AuthState != null && Session["Ethiopical_" + AuthState] == null)
                {
                    resultMessage.Message = "Session Expired";
                    resultMessage.MessageType = GenericMessages.danger;
                }

                // Check whether an error response was received from Facebook
                if (AuthError != null)
                {
                    Session.Remove("Ethiopical_" + AuthState);
                    resultMessage.Message = AuthErrorDescription;
                    resultMessage.MessageType = GenericMessages.danger;
                }

                // Redirect the user to the Facebook login dialog
                if (AuthCode == null)
                {
                    // Generate a new unique/random state
                    var state = Guid.NewGuid().ToString();

                    // Save the state in the current user session
                    Session["Ethiopical_" + state] = new[] { Callback, ContentTypeAlias, PropertyAlias };

                    // Construct the authorization URL
                    var url = client.GetAuthorizationUrl(state, "public_profile", "email"); //"user_friends"

                    // Redirect the user
                    return Redirect(url);
                }

                // Exchange the authorization code for a user access token
                var userAccessToken = string.Empty;
                try
                {
                    userAccessToken = client.GetAccessTokenFromAuthCode(AuthCode);
                }
                catch (Exception ex)
                {
                    resultMessage.Message = $"Unable to acquire access token<br/>{ex.Message}";
                    resultMessage.MessageType = GenericMessages.danger;
                }

                try
                {
                    if (string.IsNullOrEmpty(resultMessage.Message))
                    {
                        // Initialize the Facebook service (no calls are made here)
                        var service = FacebookService.CreateFromAccessToken(userAccessToken);

                        // Declare the options for the call to the API
                        var options = new FacebookGetUserOptions
                        {
                            Identifier = "me",
                            Fields = new[] { "id", "name", "email", "first_name", "last_name", "gender" }
                        };

                        var user = service.Users.GetUser(options);

                        // Try to get the email - Some FB accounts have protected passwords
                        var email = user.Body.Email;
                        if (string.IsNullOrEmpty(email))
                        {
                            resultMessage.Message = LocalizationService.GetResourceString("Members.UnableToGetEmailAddress");
                            resultMessage.MessageType = GenericMessages.danger;
                            ShowMessage(resultMessage);
                            return RedirectToAction("LogOn", "Members");
                        }

                        // First see if this user has registered already - Use email address
                        using (UnitOfWorkManager.NewUnitOfWork())
                        {
                            var userExists = MembershipService.GetUserByEmail(email);

                            if (userExists != null)
                            {
                                try
                                {
                                    // Users already exists, so log them in
                                    FormsAuthentication.SetAuthCookie(userExists.UserName, true);
                                    resultMessage.Message = LocalizationService.GetResourceString("Members.NowLoggedIn");
                                    resultMessage.MessageType = GenericMessages.success;
                                    ShowMessage(resultMessage);
                                    return RedirectToAction("Index", "Topic");
                                }
                                catch (Exception ex)
                                {
                                    LoggingService.Error(ex);
                                }
                            }
                            else
                            {
                                // Not registered already so register them
                                var viewModel = new MemberAddViewModel
                                {
                                    Email = email,
                                    LoginType = LoginType.Facebook,
                                    Password = StringUtils.RandomString(8),
                                    UserName = user.Body.Name,
                                    UserAccessToken = userAccessToken
                                };

                                // Get the image and save it
                                var getImageUrl = $"http://graph.facebook.com/{user.Body.Id}/picture?type=square";
                                viewModel.SocialProfileImageUrl = getImageUrl;

                                //Large size photo https://graph.facebook.com/{facebookId}/picture?type=large
                                //Medium size photo https://graph.facebook.com/{facebookId}/picture?type=normal
                                //Small size photo https://graph.facebook.com/{facebookId}/picture?type=small
                                //Square photo https://graph.facebook.com/{facebookId}/picture?type=square

                                // Store the viewModel in TempData - Which we'll use in the register logic
                                TempData[AppConstants.MemberRegisterViewModel] = viewModel;

                                return RedirectToAction("SocialLoginValidator", "Members");
                            }
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