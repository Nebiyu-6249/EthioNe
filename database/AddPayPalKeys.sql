/*  Localization keys for the PayPal Checkout flow (6 keys, both languages).
    Run in MSSQL Manager -> Run Query, then recycle the app pool. Idempotent + tiny:
    it inserts each key/value only if it is not already present, so it is safe to
    re-run and will never create duplicate LocaleResourceKey rows. */
SET NOCOUNT ON;
DECLARE @en uniqueidentifier = (SELECT Id FROM [Language] WHERE LanguageCulture = 'en-GB');
DECLARE @am uniqueidentifier = (SELECT Id FROM [Language] WHERE LanguageCulture = 'am-ET');
IF @en IS NULL BEGIN RAISERROR('en-GB language not found.',16,1); RETURN; END
IF @am IS NULL BEGIN RAISERROR('am-ET language not found.',16,1); RETURN; END

CREATE TABLE #kv (Name nvarchar(200) NOT NULL, Culture varchar(10) NOT NULL, Value nvarchar(1000) NOT NULL);
INSERT INTO #kv (Name,Culture,Value) VALUES
 (N'PayPal.Pay','en-GB',N'Pay with PayPal'),
 (N'PayPal.Pay','am-ET',N'በ PayPal ይክፈሉ'),
 (N'PayPal.Or','en-GB',N'or'),
 (N'PayPal.Or','am-ET',N'ወይም'),
 (N'PayPal.Success','en-GB',N'Payment successful — your ad is now active.'),
 (N'PayPal.Success','am-ET',N'ክፍያው ተሳክቷል — ማስታወቂያዎ አሁን ንቁ ነው።'),
 (N'PayPal.Cancelled','en-GB',N'PayPal payment cancelled.'),
 (N'PayPal.Cancelled','am-ET',N'የ PayPal ክፍያ ተሰርዟል።'),
 (N'PayPal.Failed','en-GB',N'PayPal payment could not be completed. Please try again.'),
 (N'PayPal.Failed','am-ET',N'የ PayPal ክፍያ ማጠናቀቅ አልተቻለም። እባክዎ እንደገና ይሞክሩ።'),
 (N'PayPal.NotConfigured','en-GB',N'PayPal is not available at the moment.'),
 (N'PayPal.NotConfigured','am-ET',N'PayPal በአሁኑ ጊዜ አይገኝም።');

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
