/*
    TASK 3 - Rebrand the live site brand text "EthioNegarit" -> "Ethiopical".

    The brand shown in the header, footer, page <title>, OG tags and About page
    all come from the DB Settings row (Html.Settings().ForumName / .PageTitle),
    NOT from code or the localization CSVs. The code seed default is already
    "Ethiopical"; this updates the EXISTING live row.

    Run in SmarterASP -> Databases -> MSSQL Manager -> Run Query, then recycle
    the app pool (settings are cached for 1 day).

    Table name note: the EF entity type is 'Settings' and the pluralizing-table
    convention is removed, so the table is [Settings]. If your install reports
    'Invalid object name', try [Setting] instead.
*/
SET NOCOUNT ON;

-- Brand name (header / footer / OG site_name / About)
UPDATE [Settings]
SET ForumName = N'Ethiopical'
WHERE ForumName IS NULL OR ForumName <> N'Ethiopical';

-- Page title, if it carried the old brand (homepage <title> uses PageTitle ?? ForumName)
UPDATE [Settings]
SET PageTitle = REPLACE(REPLACE(PageTitle, 'Ethio Negarit', 'Ethiopical'), 'EthioNegarit', 'Ethiopical')
WHERE PageTitle LIKE '%Negarit%';

-- Forum URL shown as the footer brand link / OG url, if it still points at the old domain.
-- (Safe to leave until the real domain is live; update when ethipical.com + SSL are ready.)
UPDATE [Settings]
SET ForumUrl = 'https://www.ethipical.com'
WHERE ForumUrl LIKE '%negarit%';

-- Verify
SELECT ForumName, PageTitle, ForumUrl FROM [Settings];
