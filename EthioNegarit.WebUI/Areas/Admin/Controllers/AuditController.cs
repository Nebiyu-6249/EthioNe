using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EthioNegarit.Core;
using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.WebUI.Application;
using EthioNegarit.WebUI.Areas.Admin.ViewModels;

namespace EthioNegarit.WebUI.Areas.Admin.Controllers
{
    public class AuditController : BaseAdminController
    {
        private readonly ILogAuditService _logAuditService;
        public AuditController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
          ILocalizationService localizationService, ISettingsService settingsService, ILogAuditService logAuditService) 
            : base(loggingService, unitOfWorkManager, membershipService, localizationService, settingsService)
        {
            _logAuditService = logAuditService;
        }
        // GET: Admin/Audit
        public ActionResult Index(int? p, string search)
        {
            var pageIndex = p ?? 1;
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var auditRecords = string.IsNullOrEmpty(search) ? _logAuditService.GetAll(pageIndex, SiteConstants.Instance.AdminListPageSize) :
                            _logAuditService.SearchAudit(search, pageIndex, SiteConstants.Instance.AdminListPageSize);

                var auditRecordsViewModel = new AuditRecordsViewModel
                {
                    AuditRecords = auditRecords,
                    PageIndex = pageIndex,
                    TotalCount = auditRecords.TotalCount,
                    TotalPages = auditRecords.TotalPages,
                    Search = search
                };

                return View(auditRecordsViewModel);
            }
         
        }
    }
}