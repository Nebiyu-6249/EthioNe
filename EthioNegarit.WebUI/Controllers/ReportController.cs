namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Linq;
    using Core.Domain.Entities;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using ViewModels;
    using Core.Constants;
    using Areas.Admin.ViewModels;
    using ViewModels.Mapping;
    using System.Collections.Generic;
    using Core.Domain.Enums;
    public partial class ReportController : BaseController
    {
        protected readonly IMembershipService _membershipService;
        private readonly ITopicService _topicService;
        private readonly IPostService _postService;
        private readonly IReportService _reportService;
        private readonly IMembershipUserPointsService _membershipUserPointsService;
        private readonly IChatService _chatService;
        public ReportController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            IRoleService roleService, ITopicService topicService, IPostService postService, IChatService chatService,
            ILocalizationService localizationService, ISettingsService settingsService,
            IReportService reportService, ICacheService cacheService, IMembershipUserPointsService membershipUserPointsService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _membershipService = membershipService;
            _topicService = topicService;
            _postService = postService;
            _reportService = reportService;
            _chatService = chatService;
            _membershipUserPointsService = membershipUserPointsService;
        }
        // GET: Report
        public ActionResult Index(int? p)
        {
            var settings = SettingsService.GetSettings();
            // Set the page index
            var pageIndex = p ?? 1;
            var viewModel = new List<PostReportsViewModel>();
            var reportedPosts = _postService.GetReportedPosts().ToList();
            // var reports = _reportService.GetPostReports(pageIndex, settings.TopicsPerPage, SiteConstants.Instance.ActiveTopicsListSize).ToList();
            var reports = _reportService.GetAllReports(pageIndex, settings.TopicsPerPage, SiteConstants.Instance.ActiveTopicsListSize).ToList();
            var groupedReports = reports.ToLookup(x => x.ReportedPost.Id);
            // Loop 
            foreach (var post in reportedPosts)
            {
                var id = post.Id;
                var reportsInPost = groupedReports[post.Id];
                var permissions = RoleService.GetPermissions(post.Topic.Category, UsersRole);
                var postViewModel = ViewModelMapping.CreatePostViewModel(post, post.Votes.ToList(), permissions, post.Topic, LoggedOnReadOnlyUser, SettingsService.GetSettings(), post.Favourites.ToList());
                postViewModel.MinimalPost = true;
                postViewModel.ShowTopicName = true;
                var postReports = (groupedReports.Contains(id) ? groupedReports[id].ToList() : new List<Report>());
                viewModel.Add(new PostReportsViewModel
                {
                    Post = postViewModel,
                    Reports = postReports
                });
            }
            return View(viewModel);
        }
        // [Authorize]
        public ActionResult MemberReport(Guid userId)
        {
            if (Request.IsAjaxRequest())
            {
                if (SettingsService.GetSettings().EnableMemberReporting)
                {
                    using (UnitOfWorkManager.NewUnitOfWork())
                    {
                        var user = MembershipService.GetUser(userId);
                        var viewModel = new MemberReportViewModel
                        {
                            Id = user.Id,
                            Username = user.UserName
                        };
                        return PartialView(viewModel);
                    }

                }
            }
            return null;
            // return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }
        [HttpPost]
        [Authorize]
        public void MemberReport(MemberReportViewModel viewModel)
        {
            if (SettingsService.GetSettings().EnableMemberReporting)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var user = MembershipService.GetUser(viewModel.Id);
                    var report = new Report
                    {
                        Reason = viewModel.Reason,
                        ReportedMember = user,
                        Reporter = LoggedOnReadOnlyUser
                    };
                    _reportService.MemberReport(report);
                    try
                    {
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Report.ReportSent"),
                            MessageType = GenericMessages.success
                        };
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
        }
        [HttpPost]
        //[Authorize]
        //[ValidateInput(false)]
        // [ValidateAntiForgeryToken]
        public void PostReport(CreatePostReportViewModel viewModel)
        {
            var settings = SettingsService.GetSettings();
            if (SettingsService.GetSettings().EnableSpamReporting)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    var post = _postService.Get(viewModel.PostId);
                    //Check If Logged User Enable Report and Does not Previous Report
                    var reports = _reportService.PostReports(loggedOnUser, post.Id).ToList();
                    if (reports == null || !reports.Any())
                    {
                        var reportOnUser = MembershipService.GetUser(post.User.Id);
                        var report = new Report
                        {
                            Reason = viewModel.Reason,
                            Reporter = loggedOnUser,
                            ReportedMember = reportOnUser,
                            DateReport = DateTime.UtcNow,
                            ReportedPost = post
                        };
                        report = _reportService.Add(report);
                        unitOfWork.SaveChanges();
                        //Add Points to Reporter                       
                        _membershipUserPointsService.Add(new MembershipUserPoints
                        {
                            Points = SiteConstants.Instance.PointsForReport,
                            User = loggedOnUser,
                            PointsFor = PointsFor.Report,
                            PointsForId = report.Id
                        });
                        //Check Post as its Spam
                        //Get all reports on post                      
                        var reportsOnPost = _reportService.GetReportsOnPost(post.Id);
                        if (reportsOnPost.Count() >= SiteConstants.Instance.ReportCountsToFlaggedAsSpam)
                        {
                            post.FlaggedAsSpam = true;
                            //  reportOnUser.SpamCount += 1;
                            //Points Deduct as Punishement
                            var deductPoints = -(settings.PointsDeductPerSpam);
                            _membershipUserPointsService.Add(new MembershipUserPoints
                            {
                                Points = (int)deductPoints,
                                User = reportOnUser,
                                PointsFor = PointsFor.Spam,
                                PointsForId = report.Id
                            });
                        }
                        //Get all member spam posts
                        var spamposts = _postService.GetMemberSpamPost(reportOnUser);
                        if (spamposts.Count > settings.SpamCountToBan)
                        {
                            reportOnUser.IsBanned = true;
                        }
                        //Get all Reports on member from date 
                        var datefrom = DateTime.UtcNow.AddDays(-AppConstants.TimeSpanMonthDays);
                        var allreports = _reportService.GetReportsOnUser(reportOnUser, datefrom);

                        //To Disable Posting Temporarily
                        if (allreports.Count() > SiteConstants.Instance.ReportCountToDisable)
                        {
                            reportOnUser.DisablePosting = true;
                        }
                        //To Lock Account Temporarily
                        if (allreports.Count() > settings.ReportCountsToLock)
                        {
                            reportOnUser.IsLockedOut = true;
                            reportOnUser.LockoutEndDate = DateTime.UtcNow.AddDays(1);
                        }
                        try
                        {
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }
                    }
                }
            }
        }
        //To Be rename Summary
        public ActionResult Show(Guid postId, int? p)
        {
           
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                // Set the page index
                var pageIndex = p ?? 1;
                var post = _postService.Get(postId);
                var permissions = RoleService.GetPermissions(post.Topic.Category, UsersRole);
                var postViewModel = ViewModelMapping.CreatePostViewModel(post, post.Votes.ToList(), permissions, post.Topic, LoggedOnReadOnlyUser, SettingsService.GetSettings(), post.Favourites.ToList());
                postViewModel.MinimalPost = true;
                postViewModel.ShowTopicName = true;
                // Get the reports
                var reports = _reportService.GetPostReports(pageIndex, settings.TopicsPerPage, SiteConstants.Instance.ActiveTopicsListSize, post.Id);
                var viewModel = new PostReportsViewModel
                {
                    Post = postViewModel,
                    Reports = reports,
                    PostId = postId,
                    PageIndex = pageIndex,
                    TotalCount = reports.TotalCount,
                    TotalPages = reports.TotalPages
                };
                return View(viewModel);
            }
        }
    }
}