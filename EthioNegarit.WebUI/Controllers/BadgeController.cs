namespace EthioNegarit.WebUI.Controllers
{
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using ViewModels;
    using System.Web.Mvc;
    using System;
    using Core.Domain.Entities;
    using Core.Domain.Enums;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;

    public partial class BadgeController : BaseController
    {
        private readonly IBadgeService _badgeService;
        private readonly IPostService _postService;
        private readonly IFavouriteService _favouriteService;

        public BadgeController(ILogAuditService logAuditService, ILoggingService loggingService,
            IUnitOfWorkManager unitOfWorkManager,
            IBadgeService badgeService,
            IPostService postService,
            IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService,
            ISettingsService settingsService, IFavouriteService favouriteService, ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _badgeService = badgeService;
            _postService = postService;
            _favouriteService = favouriteService;
        }
        public ActionResult Index(int? p, string search)
        {
            var pageIndex = p ?? 1;
           // var amountPerPage = int.MaxValue;
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var allBadges = string.IsNullOrEmpty(search) ? _badgeService.GetPagedGroupedBadges(pageIndex, SiteConstants.Instance.PagingGroupSize) :
                            _badgeService.SearchPagedGroupedBadges(search, pageIndex, SiteConstants.Instance.PagingGroupSize);
                foreach (var item in allBadges)
                {
                    var partialKey = string.Concat("Badge.", item.Name);
                    item.DisplayName = LocalizationService.GetResourceString(string.Concat(partialKey, ".Name"));
                    item.Description = LocalizationService.GetResourceString(string.Concat(partialKey, ".Desc"));
                }
                var viewModel = new AllBadgesViewModel
                {
                    AllBadges = allBadges,
                    PageIndex = pageIndex,
                    TotalCount = allBadges.TotalCount,
                    TotalPages = allBadges.TotalPages,
                    Search = search
                };
                return View(viewModel);
            }
        }
     
        [HttpPost]
      [Authorize]
        public void VoteUpPost(VoteBadgeViewModel voteUpBadgeViewModel)
        {
            using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.UserName);
                    var databaseUpdateNeededOne = _badgeService.ProcessBadge(BadgeType.VoteUp, loggedOnUser);
                    if (databaseUpdateNeededOne)
                    {
                        unitOfwork.SaveChanges();
                    }

                    var post = _postService.Get(voteUpBadgeViewModel.PostId);
                    var databaseUpdateNeededTwo = _badgeService.ProcessBadge(BadgeType.VoteUp, post.User);
                    if (databaseUpdateNeededTwo)
                    {
                        unitOfwork.SaveChanges();
                    }

                    if (databaseUpdateNeededOne || databaseUpdateNeededTwo)
                    {
                        unitOfwork.Commit();
                    }
                }
                catch (Exception ex)
                {
                    unitOfwork.Rollback();
                    LoggingService.Error(ex);
                }
            }
        }

        [HttpPost]
     [Authorize]
        public void VoteDownPost(VoteBadgeViewModel voteUpBadgeViewModel)
        {
            using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.UserName);
                    var databaseUpdateNeededOne = _badgeService.ProcessBadge(BadgeType.VoteDown, loggedOnUser);
                    if (databaseUpdateNeededOne)
                    {
                        unitOfwork.SaveChanges();
                    }

                    var post = _postService.Get(voteUpBadgeViewModel.PostId);
                    var databaseUpdateNeededTwo = _badgeService.ProcessBadge(BadgeType.VoteDown, post.User);

                    if (databaseUpdateNeededTwo)
                    {
                        unitOfwork.SaveChanges();
                    }

                    if (databaseUpdateNeededOne || databaseUpdateNeededTwo)
                    {
                        unitOfwork.Commit();
                    }
                }
                catch (Exception ex)
                {
                    unitOfwork.Rollback();
                    LoggingService.Error(ex);
                }
            }
        }

        [HttpPost]
     [Authorize]
        public void Post()
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                       // var user = MembershipService.Get(LoggedOnReadOnlyUser.Id);
                      //  var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.UserName);
                        var loggedOnUser = MembershipService.Get(LoggedOnReadOnlyUser.Id);
                        var databaseUpdateNeeded = _badgeService.ProcessBadge(BadgeType.Post, loggedOnUser);

                        if (databaseUpdateNeeded)
                        {
                            unitOfwork.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfwork.Rollback();
                        LoggingService.Error(ex);
                    }
                }
            }
        }

        [HttpPost]
      [Authorize]
        public void MarkAsSolution(MarkAsSolutionBadgeViewModel markAsSolutionBadgeViewModel)
        {
            using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var post = _postService.Get(markAsSolutionBadgeViewModel.PostId);
                    var databaseUpdateNeeded = _badgeService.ProcessBadge(BadgeType.MarkAsSolution, post.User) | _badgeService.ProcessBadge(BadgeType.MarkAsSolution, post.Topic.User);

                    if (databaseUpdateNeeded)
                    {
                        unitOfwork.Commit();
                    }
                }
                catch (Exception ex)
                {
                    unitOfwork.Rollback();
                    LoggingService.Error(ex);
                }
            }
        }

        [HttpPost]
       [Authorize]
        public void Favourite(FavouriteViewModel favouriteViewModel)
        {
            using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var favourite = _favouriteService.Get(favouriteViewModel.FavouriteId);
                    var databaseUpdateNeeded = _badgeService.ProcessBadge(BadgeType.Favourite, favourite.Member) | _badgeService.ProcessBadge(BadgeType.Favourite, favourite.Post.User);

                    if (databaseUpdateNeeded)
                    {
                        unitOfwork.Commit();
                    }
                }
                catch (Exception ex)
                {
                    unitOfwork.Rollback();
                    LoggingService.Error(ex);
                }
            }
        }

        [HttpPost]
    [Authorize]
        public void ProfileBadgeCheck()
        {
            using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    if (LoggedOnReadOnlyUser != null)
                    {
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.UserName);
                        var databaseUpdateNeeded = _badgeService.ProcessBadge(BadgeType.Profile, loggedOnUser);

                        if (databaseUpdateNeeded)
                        {
                            unitOfwork.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    unitOfwork.Rollback();
                    LoggingService.Error(ex);
                }
            }
        }

        [HttpPost]
        public void Time(TimeBadgeViewModel timeBadgeViewModel)
        {
            using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var user = MembershipService.GetUser(timeBadgeViewModel.Id);
                    var databaseUpdateNeeded = _badgeService.ProcessBadge(BadgeType.Time, user);

                    if (databaseUpdateNeeded)
                    {
                        unitOfwork.Commit();
                    }

                }
                catch (Exception ex)
                {
                    unitOfwork.Rollback();
                    LoggingService.Error(ex);
                }
            }
        }

        [HttpPost]
        // [Authorize]
      //  public void Point()
       public void Point(PointBadgeViewModel pointBadgeViewModel)   
        {
                using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                           var user = MembershipService.GetUser(pointBadgeViewModel.Id);
                            var databaseUpdateNeeded = _badgeService.ProcessBadge(BadgeType.Point, user);
                            if (databaseUpdateNeeded)
                            {
                                unitOfwork.Commit();
                            }
                    }
                    catch (Exception ex)
                    {
                        unitOfwork.Rollback();
                        LoggingService.Error(ex);
                    }
            }
        }



        [HttpPost]
      [Authorize]
        public void Solution()
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        if (LoggedOnReadOnlyUser != null)
                        {
                            var user = MembershipService.Get(LoggedOnReadOnlyUser.Id);
                            // var user = MembershipService.GetUser(pointBadgeViewModel.Id);
                            var databaseUpdateNeeded = _badgeService.ProcessBadge(BadgeType.Solution, user);

                            if (databaseUpdateNeeded)
                            {
                                unitOfwork.Commit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfwork.Rollback();
                        LoggingService.Error(ex);
                    }
                }
            }
        }                
    }
}







/*
public ActionResult AllBadges()
{
    using (UnitOfWorkManager.NewUnitOfWork())
    {
        var allBadges = _badgeService.GetallBadges();

        // Localise the badge names
        foreach (var item in allBadges)
        {
            var partialKey = string.Concat("Badge.", item.Name);
            item.DisplayName = LocalizationService.GetResourceString(string.Concat(partialKey, ".Name"));
            item.Description = LocalizationService.GetResourceString(string.Concat(partialKey, ".Desc"));
        }
        var badgesListModel = new AllBadgesViewModel
        {
      //  badgesListModel.AllBadges = allBadges;
          AllBadges = allBadges
        };

        return View(badgesListModel);
    }
}       */
/*
        public ActionResult MemberBadges()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var allBadges = _badgeService.GetallBadges();

                // Localise the badge names
               foreach (var item in allBadges)
                {
                    var partialKey = string.Concat("Badge.", item.Name);
                    item.DisplayName = LocalizationService.GetResourceString(string.Concat(partialKey, ".Name"));
                    item.Description = LocalizationService.GetResourceString(string.Concat(partialKey, ".Desc"));
                }

                var badgesListModel = new MemberBadgesViewModel
                {
                    MemberBadges = allBadges
                };

                return View(badgesListModel);
            }
        }
*/
