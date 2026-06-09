using System.Collections.Generic;

namespace EthioNegarit.Core.Constants
{
    public static class AppConstants
    {
        public const int SaltSize = 24;
        public const string EditorType = "forumeditor";
        // Scheduled Tasks
        public const string DefaultTaskGroup = "EthioNegaritTaskGroup";
        // Cookie names
        public const string LanguageIdCookieName = "LanguageCulture";
        public const string MemberEmailConfirmationCookieName = "EthioNegaritEmailConfirmation";
        // Cache names
        //TODO - Move to cache keys
        public const string LocalisationCacheName = "Localization-";
        public static string LanguageStrings = string.Concat(LocalisationCacheName, "LangStrings-");
        // View Bag / Temp Data Constants
        public const string MessageViewBagName = "Message";
        public const string DefaultCategoryViewBagName = "DefaultCategory";
        public const string DefaultCountry = "ET";

        public const string GlobalClass = "GlobalClass";
        public const string CurrentAction = "CurrentAction";
        public const string CurrentController = "CurrentController";      
        public const string MemberRegisterViewModel = "MemberRegisterViewModel";
        // Main admin role [This should never be changed]
        public const string AdminRoleName = "Admin";
        // Main guest role [This should never be changed]
        // This is the role a non logged in user defaults to
        public const string GuestRoleName = "Guest";

        //------------ End Permissions ----------

        // Paging options
        public const string PagingUrlFormat = "{0}?p={1}";

        // How long 
        public const int TimeSpanInMinutesToShowMembers = 12;
        public const int TimeSpanInMinutesToShowAd = 20;
        public const int TimeSpanToFreeze = 30;

        public const int TimeSpanMonthDays = 30;
        public const int TimeSpanWeekDays = 7;

        public const int ReportMaxDuration = 365;
        public const int RefundRequestPeriod = 3;
        // Notification TimeSpan
        public const int NotificationTimeSpan = 365;
        public const int PointsToEnableMessaging = 10;
        /// <summary>
        /// Last Activity Time Check. 
        /// </summary>
        public const int TimeSpanInMinutesToDoCheck = 3;

        public const int NewsEventSortOrder = 1;
        public const int TimeSpanToShowNews = 7;

        public const string EditorTemplateColourPicker = "colourpicker";
        
        //Querystring names
        public const string PostOrderBy = "order";
        public const string AllPosts = "all";


        public const string TopicViewOrder = "order";
        public const string AllTopics = "all";

        public const string PaymentViewOrder = "view";
        public const string AdViewOrder = "order";
        public const string AdViewQuery = "query";
        public const int AdReminderDaysRemains = 3;
        public const int BasicPackageSortOrder = 1;

        //Mobile Check Name
        public const string IsMobileDevice = "IsMobileDevice";

      //Month Start
        public const string January = "01/01/";
        public const string February = "02/01/";
        public const string March = "03/01/";
        public const string April = "04/01/";
        public const string May = "05/01/";
        public const string June = "06/01/";
        public const string July = "07/01/";
        public const string August = "08/01/";
        public const string September = "09/01/";
        public const string October = "10/01/";
        public const string November = "11/01/";
        public const string December = "12/01/";        
        /// <summary>
        /// Cache lengths
        /// </summary>
        public static List<string> ReflectionDllsToAvoid = new List<string>
        {
            "EcmaScript.NET.dll",
            "Unity.WebApi.dll",
            "Skybrud.Social.dll",
            "Antlr3.Runtime.dll",
            "WebGrease.dll",
            "System.Web.Optimization.dll",
            "Common.Logging.Core.dll",
            "Common.Logging.dll",
            "EntityFramework.dll",
            "EntityFramework.SqlServer.dll",
            "EntityFramework.SqlServerCompact.dll",
            "System.Data.SqlServerCe.dll",
            "Newtonsoft.Json.dll",
            "Quartz.dll",
            "SquishIt.Framework.dll",
            "SquishIt.Mvc.dll",
            "ImageProcessor.Web.dll",
            "ImageProcessor.dll",
            "AntiXssLibrary.dll",
            "HtmlSanitizationLibrary.dll",
            "System.Web.Http.dll",
            "System.Web.Http.WebHost.dll",
            "System.Net.Http.Formatting.dll",
            "System.Web.Helpers.dll",
            "System.Web.Mvc.dll",
            "System.Web.WebPages.Deployment.dll",
            "System.Web.WebPages.dll",
            "System.Web.WebPages.Razor.dll",
            "System.Web.Razor.dll",
            "Quartz.Unity.45.dll",
            "EFCache.dll",
            "HtmlAgilityPack.dll",
            "Microsoft.Practices.Unity.Configuration.dll",
            "Microsoft.Practices.Unity.dll",
            "Microsoft.Practices.Unity.RegistrationByConvention.dll",
            "Microsoft.Practices.ServiceLocation.dll",
            "Unity.WebApi.dll",
            "Unity.Mvc4.dll",
            "WebActivator.dll",
            "System.Net.Http.dll",
            "System.Net.Http.WebRequest.dll",
            "AjaxMin.dll",
            "Iesi.Collections.dll",
            "Yahoo.Yui.Compressor.dll",
            "Microsoft.Web.Services3.dll",
            "Microsoft.Web.Infrastructure.dll",
            "DotNetOpenAuth",
            "Microsoft",
        };
    }
}
