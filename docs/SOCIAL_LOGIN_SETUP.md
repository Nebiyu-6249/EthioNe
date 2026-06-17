# Social login (Google / Facebook / Microsoft) — setup & status

The app already ships a social-login implementation (Skybrud.Social based), wired into
the normal registration path: a successful provider login either logs in an existing
member (matched by email) or routes a new user through `Members/SocialLoginValidator`
→ `MemberRegisterLogic`, which creates the local `MembershipUser`, auto-approves social
accounts, and issues the existing forms-auth cookie (`EthioNegaritAuth`). The
"Sign in with …" buttons live in `Views/Shared/_SocialLoginButtons.cshtml` and already
appear on both the LogOn and Register pages.

## Status by provider
- **Google** — working after this change. The redirect URL was hard-coded to
  `http://localhost`; it now derives from `Settings.ForumUrl`
  (`GoogleOAuthController.ReturnUrl`).
- **Facebook** — already correct (redirect derives from `ForumUrl`). Just needs credentials.
- **Microsoft** — NOT enabled. The original code used the **retired Windows Live API**
  (`apis.live.net/v5.0`) and is commented out in `MicrosoftOAuthController`. It needs a
  modern rewrite (OWIN `Microsoft.Owin.Security.MicrosoftAccount` against the v2.0
  endpoint, or MSAL) before it can be turned on. Leave `MicrosoftAppId`/`Secret` blank
  and the Microsoft button stays hidden.

## SECURITY — do this first
A real Google client secret was previously committed in `App_Data/forum.config`. It has
been removed from the repo, but it is in git history, so **rotate it now** in the Google
Cloud console (delete/regenerate the OAuth client secret). Then put the new secret only
in the **server** `forum.config` (see below) — never commit it.

## Where credentials go
`App_Data/forum.config` is now **excluded from FTP deploy** (like `web.config`), so the
server copy is hand-maintained and never overwritten or committed. Edit the server file
(`h:\root\home\nebiyu6249-001\www\site1\App_Data\forum.config`) and set:

```xml
<setting key="GooglePlusAppId"     value="YOUR_GOOGLE_CLIENT_ID" />
<setting key="GooglePlusAppSecret" value="YOUR_ROTATED_GOOGLE_SECRET" />
<setting key="FacebookAppId"       value="YOUR_FACEBOOK_APP_ID" />
<setting key="FacebookAppSecret"   value="YOUR_FACEBOOK_APP_SECRET" />
```

## OAuth app registration (redirect URIs)
Register these exact "authorised redirect URIs" with each provider. They must match the
live domain in `Settings.ForumUrl` (we set it to `https://www.ethipical.com`):
- Google:   `https://www.ethipical.com/GoogleOAuth/GoogleLogin`
- Facebook: `https://www.ethipical.com/FacebookOAuth/FacebookLogin`

Providers require **HTTPS** and a real domain (they reject the `ntempurl.com` temp URL /
plain HTTP). So social login only works once `ethipical.com` + Let's Encrypt SSL are live.

## Turn it on
1. Rotate + set the credentials in the server `forum.config` (above).
2. Register the redirect URIs with each provider.
3. Run `database/EnableSocialLogins.sql` (sets `Settings.EnableSocialLogins = 1`).
4. Recycle the app pool.

The button for a provider appears only when both its AppId and AppSecret are set, so
Google/Facebook can be enabled independently and Microsoft stays hidden until rewritten.
