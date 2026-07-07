/*  Some legacy members (e.g. admin) have an uninitialised CreateDate that stored as
    SqlDateTime.MinValue (1753-01-01), which showed as "01 January 1753" as their join date.
    The app now HIDES min dates in the UI (DatesUI.GetPrettyDate), so this is optional polish:
    it backfills a real "member since" date. New members already get CreateDate = UtcNow.

    Sets any min/uninitialised CreateDate to now. If you know a member's real join date, set it
    explicitly instead. Run in MSSQL Manager -> Run Query; recycle not required (not cached).
    Table is [MembershipUser]. */
SET NOCOUNT ON;
UPDATE [MembershipUser]
SET CreateDate = GETUTCDATE()
WHERE CreateDate IS NULL OR CreateDate <= CAST('1900-01-01' AS datetime);

SELECT UserName, CreateDate FROM [MembershipUser] ORDER BY CreateDate;
