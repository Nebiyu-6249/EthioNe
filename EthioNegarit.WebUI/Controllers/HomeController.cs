namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Application;
    using ViewModels;
    using RssItem = Core.Domain.General.RssItem;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Core.Domain.Entities;
    using Core.Domain.Enums;
    using Core.Domain.General;
    using Application.CustomActionResults;
    using Core.Constants;
    using Core.Domain.Activity;
    using Application.ExtensionMethods;
    using Core.Domain;
    using ViewModels.Mapping;
    using Utilities;

    public partial class HomeController : BaseController
    {
        private readonly ITopicService _topicService;
        private readonly ICategoryService _categoryService;
        private readonly IActivityService _activityService;
        private readonly IChatService _chatService;    
        private readonly IRoleService _roleService;
        private readonly IPostService _postService;
        private readonly IReportService _reportService;
        private readonly IHelpService _helpService;
        private readonly IMembershipUserPointsService _membershipUserPointsService;
        //  private readonly string _apiToken = "202f84c1347c22"; // Store securely (e.g., in user secrets or configuration)
        public HomeController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IActivityService activityService, IMembershipService membershipService,
            ITopicService topicService, IPostService postService,ILocalizationService localizationService, IRoleService roleService,
            ISettingsService settingsService, ICategoryService categoryService, ICacheService cacheService, IReportService reportService, IChatService chatService, IHelpService helpService, IMembershipUserPointsService membershipUserPointsService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _topicService = topicService;
            _categoryService = categoryService;
            _activityService = activityService;
            _chatService = chatService;
            _roleService = roleService;
            _postService = postService;
            _reportService = reportService;
            _helpService = helpService;
            _membershipUserPointsService = membershipUserPointsService;
        }
       public ActionResult Index()
        {
            /* if (UserIsAuthenticated)
             {
                 return RedirectToAction("Index", "Topic");
                 /*   if (User.IsInRole(AppConstants.AdminRoleName)) {
                        return RedirectToAction("Index", "Admin", new { area = "Admin" });
                    }
                    else {
                    return RedirectToAction("Index", "Topic");
                }*//*
             }*/
            RecentActivity recentActivity = (RecentActivity)Session["RecentActivity"];
           if (recentActivity != null)
            {
                if (recentActivity.RecentTopics != null)
                {
                    return RedirectToAction("Index", "Topic");
                }
            }
                return View();
        }   
        public ActionResult Leaderboard()
        {
            return View();
        }
        public ActionResult Following()
        {
            return View();
        }
        public ActionResult About()
        {
            var allCats = _categoryService.GetAll();
            var viewModel = new MainStatsViewModel
            {
                LatestMembers = MembershipService.GetLatestUsers(10),
                MemberCount = MembershipService.MemberCount(),
                TopicCount = _topicService.TopicCount(allCats),
                PostCount = _postService.PostCount(allCats),
                CategoryCount = allCats.Count()
            };
            return View(viewModel);
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult PostedIn()
        {
            return View();
        }     
        public ActionResult Activity(int? p)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Set the page index
                var pageIndex = p ?? 1;		
				// Get the Activities
				var activities = _activityService.GetPagedGroupedActivities(pageIndex, SettingsService.GetSettings().ActivitiesPerPage);
                // create the view model
                var viewModel = new AllRecentActivitiesViewModel
                {
                    Activities = activities,
                    PageIndex = pageIndex,
                    TotalCount = activities.TotalCount,
                    TotalPages = activities.TotalPages
                };
                return View(viewModel);
            }
        }
        [OutputCache(Duration = (int)CacheTimes.TwoHours)]
        public ActionResult LatestRss()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Allowed Categories for a guest - As that's all we want latest RSS to show
                var guestRole = RoleService.GetRole(AppConstants.GuestRoleName);
                var allowedCategories = _categoryService.GetAllowedCategories(guestRole);

                // get an rss lit ready
                var rssTopics = new List<RssItem>();

                // Get the latest topics
                var topics = _topicService.GetRecentRssTopics(50, allowedCategories);

                // Get all the categories for this topic collection
                var categories = topics.Select(x => x.Category).Distinct();

                // create permissions
                var permissions = new Dictionary<Category, PermissionSet>();

                // loop through the categories and get the permissions
                foreach (var category in categories)
                {
                    var permissionSet = RoleService.GetPermissions(category, UsersRole);
                    permissions.Add(category, permissionSet);
                }

                // Now loop through the topics and remove any that user does not have permission for
                foreach (var topic in topics)
                {
                    // Get the permissions for this topic via its parent category
                    var permission = permissions[topic.Category];

                    // Add only topics user has permission to
                    if (!permission[SiteConstants.Instance.PermissionDenyAccess].IsTicked)
                    {
                        if (topic.Posts.Any())
                        {
                            var firstOrDefault = topic.Posts.FirstOrDefault(x => x.IsTopicStarter);
                            if (firstOrDefault != null)
                                rssTopics.Add(new RssItem { Description = firstOrDefault.PostContent, Link = topic.NiceUrl, Title = topic.Name, PublishedDate = topic.CreateDate });
                        }
                    }
                }

                return new RssResult(rssTopics, LocalizationService.GetResourceString("Rss.LatestActivity.Title"), LocalizationService.GetResourceString("Rss.LatestActivity.Description"));
            }
        }
        [OutputCache(Duration = (int)CacheTimes.TwoHours)]
        public ActionResult ActivityRss()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // get an rss lit ready
                var rssActivities = new List<RssItem>();

                var activities = _activityService.GetAll(100).OrderByDescending(x => x.ActivityMapped.Timestamp);

                var activityLink = Url.Action("Activity");

                // Now loop through the topics and remove any that user does not have permission for
                foreach (var activity in activities)
                {
                    if (activity is BadgeActivity)
                    {
                        var badgeActivity = activity as BadgeActivity;
                        rssActivities.Add(new RssItem
                        {
                            Description = badgeActivity.Badge.Description,
                            Title = string.Concat(badgeActivity.User.UserName, " ", LocalizationService.GetResourceString("Activity.UserAwardedBadge"), " ", badgeActivity.Badge.DisplayName, " ", LocalizationService.GetResourceString("Activity.Badge")),
                            PublishedDate = badgeActivity.ActivityMapped.Timestamp,
                            RssImage = AppHelpers.ReturnBadgeUrl(badgeActivity.Badge.Image),
                            Link = activityLink
                        });
                    }
                    else if (activity is MemberJoinedActivity)
                    {
                        var memberJoinedActivity = activity as MemberJoinedActivity;
                        rssActivities.Add(new RssItem
                        {
                            Description = string.Empty,
                            Title = string.Concat(memberJoinedActivity.User.UserName, " ", LocalizationService.GetResourceString("Activity.UserJoined")),
                           // Title = LocalizationService.GetResourceString("Activity.UserJoined"),
                            PublishedDate = memberJoinedActivity.ActivityMapped.Timestamp,
                            RssImage = memberJoinedActivity.User.MemberImage(SiteConstants.Instance.GravatarPostSize),
                            Link = activityLink
                        });
                    }
                    else if (activity is ProfileUpdatedActivity)
                    {
                        var profileUpdatedActivity = activity as ProfileUpdatedActivity;
                        rssActivities.Add(new RssItem
                        {
                            Description = string.Empty,
                            Title = string.Concat(profileUpdatedActivity.User.UserName, " ", LocalizationService.GetResourceString("Activity.ProfileUpdated")),
                           // Title = LocalizationService.GetResourceString("Activity.ProfileUpdated"),
                            PublishedDate = profileUpdatedActivity.ActivityMapped.Timestamp,
                            RssImage = profileUpdatedActivity.User.MemberImage(SiteConstants.Instance.GravatarPostSize),
                            Link = activityLink
                        });
                    }
                    else if (activity is TopicCreatedActivity)
                    {
                        var topicCreatedActivity = activity as TopicCreatedActivity;
                        rssActivities.Add(new RssItem
                        {
                            Description = string.Empty,
                            Title = string.Concat(topicCreatedActivity.Topic.User.UserName, " ", LocalizationService.GetResourceString("Create new discussion in"), " " , topicCreatedActivity.Topic.Category.Name),
                            PublishedDate = topicCreatedActivity.ActivityMapped.Timestamp,
                            RssImage = topicCreatedActivity.Topic.User.MemberImage(SiteConstants.Instance.GravatarPostSize),
                            Link = activityLink
                        });
                    }

                }

                return new RssResult(rssActivities, LocalizationService.GetResourceString("Rss.LatestActivity.Title"), LocalizationService.GetResourceString("Rss.LatestActivity.Description"));
            }
        }
        [OutputCache(Duration = (int)CacheTimes.TwoHours)]
        public ActionResult GoogleSitemap()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Allowed Categories for a guest
                var guestRole = RoleService.GetRole(AppConstants.GuestRoleName);
                var allowedCategories = _categoryService.GetAllowedCategories(guestRole);

                // Get all topics that a guest has access to
                var allTopics = _topicService.GetAll(allowedCategories);

                // Sitemap holder
                var sitemap = new List<SitemapEntry>();

                // ##### TOPICS
                foreach (var topic in allTopics.Where(x => x.LastPost != null))
                {
                    var sitemapEntry = new SitemapEntry
                    {
                        Name = topic.Name,
                        Url = topic.NiceUrl,
                        LastUpdated = topic.LastPost.DateEdited,
                        ChangeFrequency = SiteMapChangeFreqency.Daily,
                        Priority = "0.6"
                    };
                    sitemap.Add(sitemapEntry);
                }

                return new GoogleSitemapResult(sitemap);
            }
        }
        [OutputCache(Duration = (int)CacheTimes.TwoHours)]
        public ActionResult GoogleMemberSitemap()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // get all members profiles
                var members = MembershipService.GetAll();

                // Sitemap holder
                var sitemap = new List<SitemapEntry>();

                // #### MEMBERS
                foreach (var member in members)
                {
                    var sitemapEntry = new SitemapEntry
                    {
                        Name = member.UserName,
                        Url = member.NiceUrl,
                        LastUpdated = member.CreateDate,
                        ChangeFrequency = SiteMapChangeFreqency.Weekly,
                        Priority = "0.4"
                    };
                    sitemap.Add(sitemapEntry);
                }

                return new GoogleSitemapResult(sitemap);
            }
        }
        [OutputCache(Duration = (int)CacheTimes.TwoHours)]
        public ActionResult GoogleCategorySitemap()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Allowed Categories for a guest
                var guestRole = RoleService.GetRole(AppConstants.GuestRoleName);
                var allowedCategories = _categoryService.GetAllowedCategories(guestRole);

                // Sitemap holder
                var sitemap = new List<SitemapEntry>();

                // #### CATEGORIES
                foreach (var category in allowedCategories)
                {
                    // Get last post 
                    var topic = category.Topics.OrderByDescending(x => x.LastPost.DateEdited).FirstOrDefault();
                    var sitemapEntry = new SitemapEntry
                    {
                        Name = category.Name,
                        Url = category.NiceUrl,
                        LastUpdated = topic?.LastPost.DateEdited ?? category.DateCreated,
                        ChangeFrequency = SiteMapChangeFreqency.Monthly
                    };
                    sitemap.Add(sitemapEntry);
                }

                return new GoogleSitemapResult(sitemap);
            }
        }   
        public ActionResult Dashboard()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var accountstatus = "healthy";
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                //Get all member reports from date 
                var datefrom = DateTime.UtcNow.AddDays(-AppConstants.TimeSpanMonthDays);
                // Get Member Spam Posts
                 var spamposts = _postService.GetMemberSpamPost(loggedOnUser);
                foreach (var item in loggedOnUser.Badges)
                {
                    var partialKey = string.Concat("Badge.", item.Name);
                    item.DisplayName = LocalizationService.GetResourceString(string.Concat(partialKey, ".Name"));
                    item.Description = LocalizationService.GetResourceString(string.Concat(partialKey, ".Desc"));
                }
                var reports = _reportService.GetReportsOnUser(loggedOnUser, datefrom);
                if (reports.Count > 1)
                {
                    accountstatus = "warning";
                }
                if (reports.Count > 2)
                {
                    accountstatus = "danger";
                }
                if (reports.Count <= 1)
                {
                    accountstatus = "healthy";
                }
                return View(new DashboardViewModel
     {
         User = loggedOnUser,
                    SpamCount = spamposts.Count(),
                    ReportCount = reports.Count(),
                    AccountStatus = accountstatus,
                });
 }
}
        public ActionResult Discussions(int? p)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                var settings = SettingsService.GetSettings();
                var pageIndex = p ?? 1;
                var amountPerPage = int.MaxValue;
                var topics = _topicService.GetMembersDiscussion(pageIndex, settings.TopicsPerPage, amountPerPage,LoggedOnReadOnlyUser.Id, allowedCategories);
                var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics, RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, SettingsService.GetSettings());
                // create the view model
                var viewModel = new MemberDiscussionsViewModel
                {
                    Topics = topicViewModels,
                    PageIndex = pageIndex,
                    TotalCount = topics.TotalCount,
                    TotalPages = topics.TotalPages
                };
                return View(viewModel);
            }

        }
        public ActionResult CopyUrl(string action, string slug)
        {
            string urlToCopy = "";       
            switch (action)
            {
                case "latest":
                    urlToCopy = Url.Action("LatestRss", "Home", null, Request.Url.Scheme);
                    //   urlToCopy = Url.Action("LatestRss", "Home");
                    //   urlToCopy = string.Concat(SettingsService.GetSettings().ForumUrl.TrimEnd('/'), Url.Action("LatestRss", "Home"));
                    break;
                case "activity":                   
                    urlToCopy = Url.Action("ActivityRss", "Home", null, Request.Url.Scheme);
                    // urlToCopy = Url.Action("ActivityRss", "Home");
                    //  urlToCopy = string.Concat(SettingsService.GetSettings().ForumUrl.TrimEnd('/'), Url.Action("ActivityRss", "Home"));
                    break;
                case "category":
                    var category = _categoryService.Get(slug);
                    // check the user has permission to this category
                    var permissions = RoleService.GetPermissions(category, UsersRole);
                    if (!permissions[SiteConstants.Instance.PermissionDenyAccess].IsTicked)
                    {
                        urlToCopy = string.Concat(SettingsService.GetSettings().ForumUrl.TrimEnd('/'), Url.Action("CategoryRss", "Category", new { slug = slug }));
                    }
                    //  urlToCopy = Url.Action("ActivityRss", "Home",new {slug = slug}, Request.Url.Scheme);
                    //  urlToCopy = string.Concat(SettingsService.GetSettings().ForumUrl.TrimEnd('/'), Url.Action("ActivityRss", "Home"));
                    break;
                default:        
                    break;
            }
        //    string urlLink = string.Concat(SettingsService.GetSettings().ForumUrl.TrimEnd('/'), Url.Action(urlToCopy));
            var rssItem = new RssItem();
            rssItem.Link = urlToCopy;
            var viewModel = new CopyUrlViewModel
                   {
                       RssLink = rssItem,
                   };
            return PartialView(viewModel);
        }
        public ActionResult FAQ(int? p, string search)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var pageIndex = p ?? 1;
                var language = (LocalizationService.CurrentLanguage.LanguageCulture);
                var faqs = string.IsNullOrEmpty(search) ? _helpService.GetFAQs(language, pageIndex, SiteConstants.Instance.SearchListSize) :
                                    _helpService.Search(search, language, pageIndex, SiteConstants.Instance.SearchListSize);
                var viewModel = new FAQViewModel
                {
                    FAQ = faqs,
                    PageIndex = pageIndex,
                    TotalCount = faqs.TotalCount,
                    TotalPages = faqs.TotalPages,
                    Search = search
                };
                return View(viewModel);
            }
        }
        public PartialViewResult RssFeed()
        {
            var reader = new RssReader();
            var viewModel = new RssFeedViewModel
            {
                //   RssFeed = reader.GetRssFeed("http://feeds.smh.com.au/rssheadlines/sport.xml").Take(10).ToList()
                //  RssFeed = reader.GetRssFeed("http://localhost:53986/home/activityrss/").Take(10).ToList()
                RssFeed = reader.GetRssFeed("http://localhost:53986/home/latestrss/").Take(10).ToList()
                //   RssFeed = reader.GetRssFeed("http://localhost:53986/cat/rss/topics/").Take(10).ToList()
            };
            return PartialView(viewModel);
        }
        public CsvFileResult ExportProfile()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                return new CsvFileResult { FileDownloadName = "EthiopicalForumUser.csv", Body = MembershipService.ToCsv(LoggedOnReadOnlyUser) };
            }
        }
    }
}
