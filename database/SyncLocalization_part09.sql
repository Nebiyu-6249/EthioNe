/*  Localization import - PART 9 of 9  (801-845 of 845 keys, both languages).
    Split so each file fits SmarterASP's Run Query form (the single combined script hit
    ASP 0104 'Operation not Allowed' - a request-size limit, not a SQL error).

    Run each part once, in any order, in MSSQL Manager -> Run Query. Idempotent:
    inserts only missing LocaleResourceKey rows (matched on trimmed Name - never
    duplicates) and only missing per-language string values; existing values untouched.
    Recycle the app pool afterwards (localization is cached ~1 day).
*/
SET NOCOUNT ON;
DECLARE @en uniqueidentifier = (SELECT Id FROM [Language] WHERE LanguageCulture = 'en-GB');
DECLARE @am uniqueidentifier = (SELECT Id FROM [Language] WHERE LanguageCulture = 'am-ET');
IF @en IS NULL BEGIN RAISERROR('en-GB language not found.',16,1); RETURN; END
IF @am IS NULL BEGIN RAISERROR('am-ET language not found.',16,1); RETURN; END
CREATE TABLE #kv (Name nvarchar(200) NOT NULL, Culture varchar(10) NOT NULL, Value nvarchar(1000) NOT NULL);

INSERT INTO #kv (Name,Culture,Value) VALUES
(N'Post.TrustedUser','en-GB',N'Trusted User'),
(N'Post.TrustedUser','am-ET',N'የታመነ ተጠቃሚ,'),
(N'Post.label.TopicCategory','en-GB',N'Topic Category'),
(N'Post.label.TopicCategory','am-ET',N'ርዕስ ምድብ'),
(N'PostEdit.Panel.Name','en-GB',N'Post Edit History'),
(N'PostEdit.Panel.Name','am-ET',N'የአርትዖት ታሪክን ይለጥፉ'),
(N'PostedIn.PageTitle','en-GB',N'Topics & Posts you have posted in'),
(N'PostedIn.PageTitle','am-ET',N'እርስዎ የለጠፍካቸው ርዕሶች እና ልጥፎች'),
(N'PrivacyPolicy.HowLongRetainPersonalData','en-GB',N'How long do we retain personal data'),
(N'PrivacyPolicy.HowLongRetainPersonalData','am-ET',N'የግል መረጃዎችን ለምን ያህል ጊዜ እናቆያለን'),
(N'PrivacyPolicy.HowWeUsePersonalData','en-GB',N'How we use personal data'),
(N'PrivacyPolicy.HowWeUsePersonalData','am-ET',N'የግል መረጃዎችን እንዴት እንደምንጠቀም'),
(N'PrivacyPolicy.IntDataTransfers','en-GB',N'International data transfers'),
(N'PrivacyPolicy.IntDataTransfers','am-ET',N'ዓለም አቀፍ የውሂብ ዝውውር'),
(N'PrivacyPolicy.WhoSharePersonalData','en-GB',N'Who do wE share personal data'),
(N'PrivacyPolicy.WhoSharePersonalData','am-ET',N'የግል መረጃዎችን ለማን እናጋራለን?'),
(N'Report.PostReport','en-GB',N'Post Report'),
(N'Report.PostReport','am-ET',N'የፖስታ ሪፖርት'),
(N'Report.PostReported','en-GB',N'Post Reported In'),
(N'Report.PostReported','am-ET',N'ውስጥ ሪፖርት ተደርጓል'),
(N'Report.Report','en-GB',N'Report'),
(N'Report.Report','am-ET',N'ሪፖርት አድርግ'),
(N'Report.ReportPostBy','en-GB',N'Report Post By:'),
(N'Report.ReportPostBy','am-ET',N'ሪፖርት አድርግ በ:'),
(N'Report.ReportSent','en-GB',N'Report Sent'),
(N'Report.ReportSent','am-ET',N'ሪፖርት ተልኳል።'),
(N'SubNav.AdService','en-GB',N'Ad Service'),
(N'SubNav.AdService','am-ET',N'የማስታወቂያ አገልግሎት'),
(N'SubNav.Advertisements','en-GB',N'Advertisements'),
(N'SubNav.Advertisements','am-ET',N'ማስታወቂያዎች'),
(N'SubNav.Topics','en-GB',N'Topics'),
(N'SubNav.Topics','am-ET',N'Topics'),
(N'TermsOfService.AndAcknowledgeReadingOur','en-GB',N'and acknowledge reading our'),
(N'TermsOfService.AndAcknowledgeReadingOur','am-ET',N'እና የእኛን ንባብ እውቅና ይስጡ'),
(N'TermsOfService.Content','en-GB',N'Content'),
(N'TermsOfService.Content','am-ET',N'ይዘት'),
(N'TermsOfService.CopyrightPolicy','en-GB',N'Copyright Policy'),
(N'TermsOfService.CopyrightPolicy','am-ET',N'የቅጂ መብት ፖሊሲ'),
(N'TermsOfService.Disclaimer','en-GB',N'"AS IS" and "AS AVAILABLE" Disclaimer'),
(N'TermsOfService.Disclaimer','am-ET',N'"እንደዛ ነው" እና "እንደተገኘ" ማስተባበያ'),
(N'TermsOfService.DisputesResolution','en-GB',N'Disputes Resolution'),
(N'TermsOfService.DisputesResolution','am-ET',N'የክርክር አፈታት'),
(N'TermsOfService.EuropeanUsers','en-GB',N'For European Union (EU) Users'),
(N'TermsOfService.EuropeanUsers','am-ET',N'ለአውሮፓ ህብረት (EU) ተጠቃሚዎች'),
(N'TermsOfService.Limitation','en-GB',N'Limitation of Liability'),
(N'TermsOfService.Limitation','am-ET',N'የተጠያቂነት ገደብ'),
(N'TermsOfService.ReadAndAgree','en-GB',N'Please read and agree to the Terms and Conditions of Service carefully before using our service.'),
(N'TermsOfService.ReadAndAgree','am-ET',N'አገልግሎታችንን ከመጠቀምዎ በፊት እባክዎ የአገልግሎት ውሎች እና ሁኔታዎች በጥንቃቄ ያንብቡና ይስማሙ ።'),
(N'TermsOfService.SeverabilityandWaiver','en-GB',N'Severability and Waiver'),
(N'TermsOfService.SeverabilityandWaiver','am-ET',N'መቆራረጥ እና መሻር'),
(N'TermsOfService.USLegalCompliance','en-GB',N'United States Legal Compliance'),
(N'TermsOfService.USLegalCompliance','am-ET',N'የዩናይትድ ስቴትስ የሕግ ተገዢነት'),
(N'TermsOfService.YouAgree','en-GB',N'You Agree'),
(N'TermsOfService.YouAgree','am-ET',N'መለያ ፍጠርን በመምረጥ፣ ጋር ተስማምተዋል'),
(N'Topic.AlreadyAnswered','en-GB',N'Questions that may already have your answer'),
(N'Topic.AlreadyAnswered','am-ET',N'ምናልባት የእርስዎ መልስ ሊኖራቸው የሚችሉ ጥያቄዎች'),
(N'Topic.Label.TopicTitle','en-GB',N'Title'),
(N'Topic.Label.TopicTitle','am-ET',N'ርዕስ'),
(N'Topic.ShowMorePosts','en-GB',N'Show More Posts'),
(N'Topic.ShowMorePosts','am-ET',N'ተጨማሪ ልጥፎችን አሳይ'),
(N'Topic.ShowMorePostsLoading','en-GB',N'Loading Posts...'),
(N'Topic.ShowMorePostsLoading','am-ET',N'ልጥፎችን በመጫን ላይ...'),
(N'Chat.AllRooms','en-GB',N'All rooms'),
(N'Chat.AllRooms','am-ET',N'ሁሉም ክፍሎች'),
(N'Chat.Connected','en-GB',N'Connected'),
(N'Chat.Connected','am-ET',N'ተገናኝቷል'),
(N'Chat.Connecting','en-GB',N'Connecting…'),
(N'Chat.Connecting','am-ET',N'በመገናኘት ላይ…'),
(N'Chat.ConnectFailed','en-GB',N'Could not connect. Refresh to retry.'),
(N'Chat.ConnectFailed','am-ET',N'መገናኘት አልተቻለም። ለመድገም ያድሱ።'),
(N'Chat.Send','en-GB',N'Send'),
(N'Chat.Send','am-ET',N'ላክ'),
(N'Chat.SignInToChat','en-GB',N'Please sign in to join the conversation.'),
(N'Chat.SignInToChat','am-ET',N'ለመወያየት እባክዎ ይግቡ።'),
(N'Chat.NoMessages','en-GB',N'No messages yet. Be the first to say something.'),
(N'Chat.NoMessages','am-ET',N'እስካሁን መልዕክት የለም። መጀመሪያ ይሁኑ።'),
(N'Chat.Reply','en-GB',N'Reply'),
(N'Chat.Reply','am-ET',N'መልስ ስጥ'),
(N'Chat.Like','en-GB',N'Like'),
(N'Chat.Like','am-ET',N'ወድጄዋለሁ'),
(N'Chat.Flag','en-GB',N'Flag'),
(N'Chat.Flag','am-ET',N'ምልክት አድርግ'),
(N'Chat.Online','en-GB',N'online'),
(N'Chat.Online','am-ET',N'መስመር ላይ'),
(N'Chat.ReplyingTo','en-GB',N'Replying to {0}'),
(N'Chat.ReplyingTo','am-ET',N'ለ{0} በመመለስ ላይ'),
(N'Chat.MessageDeleted','en-GB',N'message deleted'),
(N'Chat.MessageDeleted','am-ET',N'መልዕክት ተሰርዟል'),
(N'Chat.CancelReply','en-GB',N'Cancel reply'),
(N'Chat.CancelReply','am-ET',N'መልስ ሰርዝ');

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
