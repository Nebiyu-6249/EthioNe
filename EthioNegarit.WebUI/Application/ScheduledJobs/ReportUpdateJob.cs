using System;
using Quartz;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Core.Interfaces.UnitOfWork;

namespace EthioNegarit.WebUI.Application.ScheduledJobs
{
    [DisallowConcurrentExecution]
    public class ReportUpdateJob : IJob
    {
        private readonly ILoggingService _loggingService;
        private readonly IReportService _reportService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ReportUpdateJob(ILoggingService loggingService, IReportService reportService, IUnitOfWorkManager unitOfWorkManager)
        {
            _loggingService = loggingService;
            _reportService = reportService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void Execute(IJobExecutionContext context)
        {
            using (var unitOfWork = _unitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    // Process emails to send - Only send the amount per job from the siteconstants
                    _reportService.UpdateReports();
                    // Commit - Which deletes the jobs that have been sent
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    _loggingService.Error(ex);
                }
            }

        }
    }
}