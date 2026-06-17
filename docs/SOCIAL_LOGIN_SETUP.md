# Social login (Google / Microsoft / Facebook) — OWIN setup

> This branch (`claude/social-login-owin`) replaces the older Skybrud.Social flow with
> modern **OWIN external authentication** (Katana 3.0.1) for all three providers,
> including Microsoft (the Skybrud version used the retired Windows Live API). It is on a
> separate branch so a build issue can't affect the live `main`. Merge it once CI is green
> and you've configured at least one provider.

## How it works
- `Startup.cs` wires `UseCookieAuthentication` + `UseGoogleAuthentication` /
  `UseMicrosoftAccountAuthentication` / `UseFacebookAuthentication`. Each provider is added
  ONLY when its id+secret are present in Web.config `<appSettings>`, so with blank settings
  nothing changes at startup.
- `AuthController.ExternalLogin(provider)` starts the challenge; the provider returns to the
  OWIN middleware callback (`/signin-google`, `/signin-microsoft`, `/signin-facebook`), then
  to `AuthController.ExternalLoginCallback`.
- The callback matches the external email to a local member: if found, it issues the normal
  forms-auth cookie (`EthioNegaritAuth`); if not, it routes through the existing
  `Members/SocialLoginValidator` → `MemberRegisterLogic`, which creates + auto-approves the
  account and logs them in. No ASP.NET Identity is introduced.
- Buttons are in `Views/Shared/_SocialLoginButtons.cshtml` (already on LogOn + Register) and
  show only when `Settings.EnableSocialLogins` is on AND the provider's credentials exist.

## SECURITY — do this first
A real Google client secret was previously committed in `App_Data/forum.config`. It has
been removed from the repo but remains in git history — **rotate it** in the Google Cloud
console. Put the new secret only in the **server** Web.config (below); never commit secrets.

## Where credentials go (server Web.config `<appSettings>`)
`Startup` reads credentials from Web.config app settings (not forum.config, not the DI
container — both are unreliable that early in startup). Web.config is excluded from FTP, so
these live only on the server and are never overwritten or committed. Add to
`h:\root\home\nebiyu6249-001\www\site1\Web.config`:
```xml
<add key="GooglePlusAppId"     value="YOUR_GOOGLE_CLIENT_ID" />
<add key="GooglePlusAppSecret" value="YOUR_ROTATED_GOOGLE_SECRET" />
<add key="MicrosoftAppId"      value="YOUR_MICROSOFT_CLIENT_ID" />
<add key="MicrosoftAppSecret"  value="YOUR_MICROSOFT_CLIENT_SECRET" />
<add key="FacebookAppId"       value="YOUR_FACEBOOK_APP_ID" />
<add key="FacebookAppSecret"   value="YOUR_FACEBOOK_APP_SECRET" />
```
(The same key names appear commented in the repo Web.config for reference.)

## OAuth app registration — redirect URIs
Register these exact redirect URIs with each provider (HTTPS, real domain — providers reject
the `ntempurl.com` temp URL and plain HTTP). Use the live domain (`ethipical.com`):
- Google:    `https://www.ethipical.com/signin-google`
- Microsoft: `https://www.ethipical.com/signin-microsoft`
- Facebook:  `https://www.ethipical.com/signin-facebook`

These `/signin-*` paths are the OWIN middleware's default callback paths (handled
automatically; you don't add routes for them).

## Turn it on
1. Rotate + set the credentials in the server Web.config (above).
2. Register the redirect URIs with each provider.
3. Run `database/EnableSocialLogins.sql` (sets `Settings.EnableSocialLogins = 1`).
4. Recycle the app pool (touch `/site1/Web.config`).

Social login is testable only once `ethipical.com` + SSL are live. A provider's button
appears only when both its id and secret are set, so you can enable providers one at a time.

---

## Per-provider registration walkthrough
For each provider, create an OAuth app to get the Client/App **ID + Secret**, register the
**exact** redirect URI below, then put the id+secret in the server `Web.config` appSettings
keys named above. Redirect URIs must match character-for-character (https, `www`, no trailing
slash). All three require the live HTTPS domain — none work on the HTTP temp URL.

### Google  → keys `GooglePlusAppId` / `GooglePlusAppSecret`
1. https://console.cloud.google.com → create/select a project.
2. **APIs & Services → OAuth consent screen**: choose **External**, fill app name, support
   email, your domain; add scopes `openid`, `email`, `profile`. While in **Testing** mode add
   your own Google account under **Test users** (or **Publish** the app for anyone to sign in).
3. **APIs & Services → Credentials → Create credentials → OAuth client ID → Web application**.
   - Authorised redirect URI: `https://www.ethipical.com/signin-google`
4. Copy the **Client ID** → `GooglePlusAppId`, **Client secret** → `GooglePlusAppSecret`.

### Microsoft  → keys `MicrosoftAppId` / `MicrosoftAppSecret`
1. https://entra.microsoft.com (or portal.azure.com) → **App registrations → New registration**.
2. Supported account types: **Accounts in any organizational directory and personal Microsoft
   accounts** (so consumer @outlook/@hotmail works).
3. **Redirect URI**: platform **Web** = `https://www.ethipical.com/signin-microsoft`.
4. **Certificates & secrets → New client secret** → copy the secret **Value** (not the Id).
5. **Application (client) ID** → `MicrosoftAppId`, secret value → `MicrosoftAppSecret`.

### Facebook  → keys `FacebookAppId` / `FacebookAppSecret`
1. https://developers.facebook.com → **My Apps → Create App** (type: **Consumer**).
2. Add the **Facebook Login** product → **Settings**.
   - Valid OAuth Redirect URIs: `https://www.ethipical.com/signin-facebook`
3. **Settings → Basic**: **App ID** → `FacebookAppId`, **App Secret** → `FacebookAppSecret`.
4. Facebook only returns email to a **Live** app; flip the app from Development to **Live**
   (a basic app with email/public_profile usually needs no full review, but the app must be Live
   and you must add a Privacy Policy URL — `https://www.ethipical.com/privacy`).

## What happens on sign-in (the bridge, already coded)
`AuthController.ExternalLoginCallback` reads the provider's email/name, then:
- if a member with that email exists → logs them in (issues the `EthioNegaritAuth` forms cookie);
- otherwise → routes through `Members/SocialLoginValidator` → `MemberRegisterLogic`, which creates
  the local `MembershipUser`, auto-approves the social account, assigns the default role/profile,
  and logs them in. No password is needed by the user (a random one is stored).
Button labels are localized (`Members.Logon{Google,Microsoft,Facebook}Button`, en + am).
