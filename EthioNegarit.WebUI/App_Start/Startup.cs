using System;
using System.Configuration;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.MicrosoftAccount;

[assembly: OwinStartup(typeof(EthioNegarit.WebUI.App_Start.Startup))]
//[assembly: OwinStartup(typeof(SignalRChat.Startup))]

namespace EthioNegarit.WebUI.App_Start
    {
    public class Startup
    {
        // The external sign-in cookie type. AuthController reads it back in the callback.
        public const string ExternalCookieType = "ExternalCookie";

        public void Configuration(IAppBuilder app)
        {
            // External (social) auth must be wired before SignalR so the auth cookie
            // middleware is in the pipeline for the OAuth round-trip.
            ConfigureExternalAuth(app);

            // Map the SignalR service
            app.MapSignalR();
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }

        // Reads provider credentials from Web.config <appSettings> (NOT forum.config and NOT
        // the DI container): ConfigurationManager is always available at OWIN startup, whereas
        // SiteConstants resolves IConfigService through DependencyResolver, which may not be
        // wired yet this early in app start. Web.config is also excluded from FTP, so the
        // secrets live only on the server and are never committed. Each provider is added
        // ONLY when both its id and secret are present, so with blank settings this whole
        // method is a no-op and cannot affect startup.
        private static void ConfigureExternalAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(ExternalCookieType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = ExternalCookieType,
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = ".AspNet.ExternalCookie",
                ExpireTimeSpan = TimeSpan.FromMinutes(10)
            });

            var googleId = AppSetting("GooglePlusAppId");
            var googleSecret = AppSetting("GooglePlusAppSecret");
            if (!string.IsNullOrEmpty(googleId) && !string.IsNullOrEmpty(googleSecret))
            {
                app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
                {
                    ClientId = googleId,
                    ClientSecret = googleSecret
                });
            }

            var microsoftId = AppSetting("MicrosoftAppId");
            var microsoftSecret = AppSetting("MicrosoftAppSecret");
            if (!string.IsNullOrEmpty(microsoftId) && !string.IsNullOrEmpty(microsoftSecret))
            {
                app.UseMicrosoftAccountAuthentication(new MicrosoftAccountAuthenticationOptions
                {
                    ClientId = microsoftId,
                    ClientSecret = microsoftSecret
                });
            }

            var facebookId = AppSetting("FacebookAppId");
            var facebookSecret = AppSetting("FacebookAppSecret");
            if (!string.IsNullOrEmpty(facebookId) && !string.IsNullOrEmpty(facebookSecret))
            {
                app.UseFacebookAuthentication(new FacebookAuthenticationOptions
                {
                    AppId = facebookId,
                    AppSecret = facebookSecret
                });
            }
        }

        private static string AppSetting(string key)
        {
            try { return ConfigurationManager.AppSettings[key] ?? string.Empty; }
            catch { return string.Empty; }
        }
    }
}
