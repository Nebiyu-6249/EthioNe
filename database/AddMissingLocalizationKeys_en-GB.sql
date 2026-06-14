/*
    Add missing English (en-GB) localization strings that currently render as raw
    resource keys on the site (e.g. "Text.Copyright", "Help.FAQ", "About.Discussions").

    Run in SmarterASP control panel -> Databases -> MSSQL Manager -> Run Query.

    Safe to run more than once:
      - a resource KEY is only created when no key with that (trimmed) Name exists,
        so it can never re-introduce the duplicate-key rows that crashed localization;
      - an en-GB STRING value is only created when that key has no en-GB value yet.

    After running, recycle the app pool (or wait for the 1-day localization cache to
    expire) so the new strings are picked up.
*/
SET NOCOUNT ON;

-- The site's default language is English (United Kingdom) = en-GB.
-- (Run  SELECT Id, Name, LanguageCulture FROM [Language];  to confirm.)
DECLARE @langId uniqueidentifier;
SELECT @langId = Id FROM [Language] WHERE LanguageCulture = 'en-GB';

IF @langId IS NULL
BEGIN
    RAISERROR('en-GB language row not found. Check [Language].LanguageCulture and adjust this script.', 16, 1);
    RETURN;
END

-- Key name + the English text to show
DECLARE @kv TABLE (Name nvarchar(200) PRIMARY KEY, Value nvarchar(1000));
INSERT INTO @kv (Name, Value) VALUES
 ('About.Discussions',      'Discussions'),
 ('About.CategoriesText',   'Categories'),
 ('About.MembersText',      'Members'),
 ('About.TopicsText',       'Topics'),
 ('Home.Permalinks',        'Quick Links'),
 ('Layout.Nav.Discussions', 'Discussions'),
 ('Layout.Nav.Help',        'Help'),
 ('Help.FAQ',               'FAQ'),
 ('SubNav.AdService',       'Ad Service'),
 ('Home.PrivacyAndSecurity','Privacy and Security'),
 ('Home.ContactUs',         'Contact Us'),
 ('Text.Copyright',         'Copyright'),
 ('Text.AllRights',         'All rights reserved.'),
 ('Text.Version',           'Version');

-- 1) Create resource keys that don't already exist (matched on trimmed Name)
INSERT INTO LocaleResourceKey (Id, Name, Notes, DateAdded)
SELECT NEWID(), kv.Name, NULL, GETUTCDATE()
FROM @kv kv
WHERE NOT EXISTS (
    SELECT 1 FROM LocaleResourceKey k WHERE LTRIM(RTRIM(k.Name)) = kv.Name
);

-- 2) Add the en-GB value for each key that has no en-GB value yet.
--    OneKeyPerName guards against any pre-existing duplicate key rows so we
--    never insert more than one string per key.
;WITH OneKeyPerName AS (
    SELECT TrimName = LTRIM(RTRIM(Name)),
           Id,
           rn = ROW_NUMBER() OVER (PARTITION BY LTRIM(RTRIM(Name)) ORDER BY DateAdded, Id)
    FROM LocaleResourceKey
)
INSERT INTO LocaleStringResource (Id, ResourceValue, LocaleResourceKey_Id, Language_Id)
SELECT NEWID(), kv.Value, k.Id, @langId
FROM @kv kv
INNER JOIN OneKeyPerName k ON k.TrimName = kv.Name AND k.rn = 1
WHERE NOT EXISTS (
    SELECT 1 FROM LocaleStringResource s
    WHERE s.LocaleResourceKey_Id = k.Id AND s.Language_Id = @langId
);

-- Report what now exists for these keys
SELECT k.Name, s.ResourceValue
FROM @kv kv
INNER JOIN LocaleResourceKey k ON LTRIM(RTRIM(k.Name)) = kv.Name
LEFT JOIN LocaleStringResource s ON s.LocaleResourceKey_Id = k.Id AND s.Language_Id = @langId
ORDER BY k.Name;
