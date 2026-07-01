

namespace EthioNegarit.Services
    {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core.Constants;
    using Core.Domain.Entities;
    //  using Core.Domain.General;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    using Utilities;
    using System.Data.Entity;
    using Core.Domain.General;
    using Core.Domain;

    public partial class ReportService : IReportService
        {
        private readonly EthioNegaritContext _context;
        private readonly IEmailService _emailService;
        private readonly ISettingsService _settingsService;
        private readonly ILocalizationService _localizationService;
        private readonly ILoggingService _loggingService;
        //  private readonly IChatService _chatService;
        private readonly ICacheService _cacheService;

        public ReportService (IEthioNegaritContext context, IEmailService emailService, ISettingsService settingsService, ILocalizationService localizationService, ILoggingService loggingService, ICacheService cacheService)
            {
            _emailService = emailService;
            _settingsService = settingsService;
            _loggingService = loggingService;
            _cacheService = cacheService;
            // _chatService = chatService;
            _localizationService = localizationService;
            _context = context as EthioNegaritContext;
            }

        public Report Get (Guid reportId)
            {
            // var cacheKey = string.Concat(CacheKeys.Report.StartsWith, "Get-", reportId);
            // return _cacheService.CachePerRequest(cacheKey, () =>
            return (_context.Report
                // .Include(x => x.Users)
                .Include(x => x.Reporter)
                .Include(x => x.ReportedMember)
                .FirstOrDefault(x => x.Id == reportId)

                );
            }
        public IEnumerable<Report> GetByGuid (Guid referenceId)
            {
            var cacheKey = string.Concat(CacheKeys.Report.StartsWith, "GetByGuid-", referenceId);
            // return _cacheService.CachePerRequest(cacheKey, () => _context.MemberReport.Where(x referenceId.Contains(x.ReferenceId)));
            return _cacheService.CachePerRequest(cacheKey, () => _context.Report.Where(x => x.Id == referenceId));
            // .Where(x => x.Type == type && dataId.Contains(x.DataId))
            }
        public Report Add (Report report)
            {
            return _context.Report.Add(report);
            }
        public void Delete (Report report)
            {
            _context.Report.Remove(report);
            }
        public IEnumerable<Report> GetAll ()
            {
            var cacheKey = string.Concat(CacheKeys.Report.StartsWith, "GetAll");
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.Report
                .ToList();
            });
            }
        public List<Report> GetAllReports ()
            {
            return _context.Report.OrderBy(x => x.DateReport).ToList();
            }
        public IList<Report> PostReports (MembershipUser user, Guid postId)
            {
            var cacheKey = string.Concat(CacheKeys.Report.StartsWith, "PostReport-", user.Id, "-", postId);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var reports = _context.Report.Where(x => x.ReportedPost.Id == postId && x.Reporter.Id == user.Id);
                return reports.ToList();
            });
            }
        public void UpdateReports ()
            {
            try
                {
                //Get All Reports
                var reports = GetAllReports();
                if (reports != null && reports.Any())
                    {
                    var reportsToDelete = new List<Report>();
                    foreach (var report in reports)
                        {
                        //Check if Date End
                        var dateRemove = DateTime.UtcNow.AddDays(-AppConstants.ReportMaxDuration);
                        if (report.DateReport > dateRemove)
                            {
                            reportsToDelete.Add(report);
                            }
                        }

                    // Loop through the Date end reports and delete them
                    foreach (var report in reportsToDelete)
                        {
                        //  Delete(report);
                        }
                    }
                }
            catch (Exception ex)
                {
                _loggingService.Error(ex);
                }
            }
        public void MemberReport (Report report)
            {
            var sb = new StringBuilder();
            var email = new Email();

            sb.AppendFormat("<p>{2}: <a href=\"{0}\">{1}</a></p>",
                string.Concat(_settingsService.GetSettings().ForumUrl.TrimEnd('/'), report.Reporter.NiceUrl),
                report.Reporter.UserName,
                _localizationService.GetResourceString("Report.Reporter"));

            sb.AppendFormat("<p>{2}: <a href=\"{0}\">{1}</a></p>",
                string.Concat(_settingsService.GetSettings().ForumUrl.TrimEnd('/'), report.ReportedMember.NiceUrl),
                report.ReportedMember.UserName,
                _localizationService.GetResourceString("Report.MemberReported"));

            sb.Append($"<p>{_localizationService.GetResourceString("Report.Reason")}:</p>");
            sb.Append($"<p>{report.Reason}</p>");

            email.EmailTo = _settingsService.GetSettings().AdminEmailAddress;
            email.Subject = _localizationService.GetResourceString("Report.MemberReport");
            email.NameTo = _localizationService.GetResourceString("Report.Admin");

            email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
            _emailService.SendMail(email);
            }
        public IList<Report> GetRecentReports (int amountToTake, DateTime? from)
            {
            if (from == null)
                {
                from = DateTime.UtcNow.AddDays(-14);
                }
            return _context.Report
                                .Include(x => x.ReportedPost)
                                .Include(x => x.Reporter)
                                .Include(x => x.ReportedMember)
                                .AsNoTracking()
                            .Where(x => x.DateReport >= from)
                            .OrderBy(x => x.DateReport)
                            .Take(amountToTake)
                            .ToList();
            }
        public PagedList<Report> GetAllReports(int pageIndex, int pageSize, int amountToTake)
        {
            var cacheKey = string.Concat(CacheKeys.Report.StartsWith, "GetAllReports-", pageIndex, "-", pageSize, "-", amountToTake);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var total = _context.Report.AsNoTracking().Count();
                if (amountToTake < total)
                {
                    total = amountToTake;
                }
                // Get the topics using an efficient
                var results = _context.Report
                                    .AsNoTracking()
                                     .Include(x => x.ReportedPost)
                                .Include(x => x.Reporter)
                                .Include(x => x.ReportedMember)
                                .OrderBy(x => x.DateReport)
                                 .Take(pageSize)
                                .Skip((pageIndex - 1) * pageSize)
                                .ToList();


                // var reports = results.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                // Return a paged list
                return new PagedList<Report>(results, pageIndex, pageSize, total);
            });
        }
        public PagedList<Report> GetPostReports(int pageIndex, int pageSize, int amountToTake, Guid postId)
            {
            var cacheKey = string.Concat(CacheKeys.Report.StartsWith, "GetPostReports-", pageIndex, "-", pageSize, "-", amountToTake, "-", postId);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var total = _context.Report.AsNoTracking().Count(x => x.ReportedPost.Id == postId);
                if (amountToTake < total)
                    {
                    total = amountToTake;
                    }
                var results = _context.Report
                                    .AsNoTracking()
                                     .Include(x => x.ReportedPost)
                                .Include(x => x.Reporter)
                                .Include(x => x.ReportedMember)
                                .Where(x => x.ReportedPost.Id == postId)
                                .OrderBy(x => x.DateReport)
                                 .Take(pageSize)
                                .Skip((pageIndex - 1) * pageSize)
                                .ToList();
                return new PagedList<Report>(results, pageIndex, pageSize, total);
            });
            }   
        public IList<Report> GetReportsOnPost (Guid postId)
            {
            return _context.Report
                                .Include(x => x.ReportedPost)
                                .Include(x => x.Reporter)
                                .Include(x => x.ReportedMember)
                                .AsNoTracking()
                            .Where(x => x.ReportedPost.Id == postId)
                            .OrderBy(x => x.DateReport)
                            .ToList();
            }
        public IList<Report> GetReportsOnUser (MembershipUser user, DateTime? from)
            {
            if (from == null)
                {
                from = DateTime.UtcNow.AddDays(-SiteConstants.Instance.ReportMaxWaitingDays);
                }
            return _context.Report
                                .Include(x => x.ReportedPost)
                                .Include(x => x.Reporter)
                                .Include(x => x.ReportedMember)
                                .AsNoTracking()
                            .Where(x => x.ReportedMember.Id == user.Id && x.DateReport > from)
                            .OrderBy(x => x.DateReport)
                            .ToList();
            }
    }
    }












/*
        public PagedList<Report> GetReports (int pageIndex, int pageSize, int amountToTake)
            {
            var cacheKey = string.Concat(CacheKeys.Report.StartsWith, "GetReports-", pageIndex, "-", pageSize, "-", amountToTake);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                // We might only want to display the top 100
                // but there might not be 100 reports
                var total = _context.Report.AsNoTracking().Count();
                if (amountToTake < total)
                    {
                    total = amountToTake;
                    }
                // Get the topics using an efficient
                var results = _context.Report
                                    .AsNoTracking()
                                     .Include(x => x.ReportedPost)
                                .Include(x => x.Reporter)
                                .Include(x => x.ReportedMember)

                                .OrderBy(x => x.DateReport);

                //  .Where(x => x.Pending != true && allowedCatIds.Contains(x.Category.Id));

                // sort the paging out
                var reports = results.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                // Return a paged list
                return new PagedList<Report>(reports, pageIndex, pageSize, total);
            });
            }
*/
