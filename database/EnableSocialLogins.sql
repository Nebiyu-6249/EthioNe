/*
    TASK 5 - turn the social-login buttons on.

    The Google / Facebook / Microsoft buttons on the LogOn and Register pages are
    gated on Settings.EnableSocialLogins. They also only render for a provider whose
    AppId + AppSecret are present in the SERVER App_Data/forum.config (see
    docs/SOCIAL_LOGIN_SETUP.md). Run this once the credentials are in place, then
    recycle the app pool (settings are cached for 1 day).

    Table is [Settings] (EF type name; pluralization convention removed). If your
    install reports 'Invalid object name', try [Setting].
*/
UPDATE [Settings] SET EnableSocialLogins = 1;
SELECT EnableSocialLogins FROM [Settings];
