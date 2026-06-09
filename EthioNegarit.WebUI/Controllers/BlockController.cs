namespace EthioNegarit.WebUI.Controllers
{
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Core.Constants;
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using ViewModels;
    using Core.Domain.Entities;
    public partial class BlockController : BaseController
    {
        // GET: Block
        private readonly IBlockService _blockService;
        public BlockController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, IBlockService blockService, ICacheService cacheService)
            :base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _blockService = blockService;
        }
        public ActionResult Index(int? p)
        {
                var pageIndex = p ?? 1;
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Check for online status
                var date = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanInMinutesToShowMembers);
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                var membersBlocked = _blockService.GetBlockedMembers(pageIndex, SiteConstants.Instance.AdminListPageSize, loggedOnUser.Id);
                var blockedMembers = membersBlocked.Select(x => x.Blocked).ToList();
                var viewModel = new PublicMemberListViewModel
                {
                 //   Users = allViewModelUsers,
                    Members = blockedMembers,
                    PageIndex = pageIndex,
                    TotalCount = membersBlocked.Count,
                    TotalPages = membersBlocked.TotalPages

                };
                return View(viewModel);
            }
        }
        [HttpPost]
        [Authorize]
        public virtual void BlockOrUnBlock(BlockMemberViewModel viewModel)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Get a db user
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);

                        // Other member
                        var otherMember = MembershipService.GetUser(viewModel.MemberToBlockOrUnBlock);

                        var block = loggedOnUser.BlockedUsers.FirstOrDefault(x => x.Blocked.Id == otherMember.Id);
                        if (block != null)
                        {
                            var getBlock = _blockService.Get(block.Id);
                            _blockService.Delete(getBlock);
                        }
                        else
                        {
                            loggedOnUser.BlockedUsers.Add(new Block
                            {
                                Blocked = otherMember,
                                Blocker = loggedOnUser,
                                Date = DateTime.UtcNow
                            });
                        }

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
        }     
    }
}
