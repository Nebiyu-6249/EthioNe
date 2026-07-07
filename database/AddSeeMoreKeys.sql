/*  Localization keys for the See-More / See-Less expander (2 keys, both languages).
    Run in MSSQL Manager -> Run Query, then recycle. Idempotent + tiny. */
SET NOCOUNT ON;
DECLARE @en uniqueidentifier = (SELECT Id FROM [Language] WHERE LanguageCulture = 'en-GB');
DECLARE @am uniqueidentifier = (SELECT Id FROM [Language] WHERE LanguageCulture = 'am-ET');
IF @en IS NULL BEGIN RAISERROR('en-GB language not found.',16,1); RETURN; END
IF @am IS NULL BEGIN RAISERROR('am-ET language not found.',16,1); RETURN; END

CREATE TABLE #kv (Name nvarchar(200) NOT NULL, Culture varchar(10) NOT NULL, Value nvarchar(1000) NOT NULL);
INSERT INTO #kv (Name,Culture,Value) VALUES
 (N'SeeMore.More','en-GB',N'See More'),
 (N'SeeMore.More','am-ET',N'ተጨማሪ አሳይ'),
 (N'SeeMore.Less','en-GB',N'See Less'),
 (N'SeeMore.Less','am-ET',N'አሳንስ');

INSERT INTO LocaleResourceKey (Id, Name, Notes, DateAdded)
SELECT NEWID(), x.Name, NULL, GETUTCDATE()
FROM (SELECT DISTINCT Name FROM #kv) x
WHERE NOT EXISTS (SELECT 1 FROM LocaleResourceKey k WHERE LTRIM(RTRIM(k.Name)) = x.Name);

;WITH OneKeyPerName AS (
    SELECT TrimName = LTRIM(RTRIM(Name)), Id,
           rn = ROW_NUMBER() OVER (PARTITION BY LTRIM(RTRIM(Name)) ORDER BY DateAdded, Id)
    FROM LocaleResourceKey
)
INSERT INTO LocaleStringResource (Id, ResourceValue, LocaleResourceKey_Id, Language_Id)
SELECT NEWID(), kv.Value, k.Id, CASE kv.Culture WHEN 'en-GB' THEN @en ELSE @am END
FROM #kv kv
INNER JOIN OneKeyPerName k ON k.TrimName = kv.Name AND k.rn = 1
WHERE NOT EXISTS (SELECT 1 FROM LocaleStringResource s
    WHERE s.LocaleResourceKey_Id = k.Id
      AND s.Language_Id = CASE kv.Culture WHEN 'en-GB' THEN @en ELSE @am END);
DROP TABLE #kv;
