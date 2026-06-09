using System;
using Quartz;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Core.Interfaces.UnitOfWork;

namespace EthioNegarit.WebUI.Application.ScheduledJobs
{
  
    [DisallowConcurrentExecution]
    public class EmailJob : IJob
    {
        private readonly ILoggingService _loggingService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public EmailJob(ILoggingService loggingService, IEmailService emailService, IUnitOfWorkManager unitOfWorkManager)
        {
            _loggingService = loggingService;
            _emailService = emailService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void Execute(IJobExecutionContext context)
        {
            using (var unitOfWork = _unitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    // Process emails to send - Only send the amount per job from the siteconstants
                    _emailService.ProcessMail(5);

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