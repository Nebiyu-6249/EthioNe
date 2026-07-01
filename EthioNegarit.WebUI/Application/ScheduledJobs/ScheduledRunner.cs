using EthioNegarit.Core.Constants;
using Microsoft.Practices.Unity;
using Quartz;

namespace EthioNegarit.WebUI.Application.ScheduledJobs
{
    public static class ScheduledRunner
    {
        public static void Run(IUnityContainer container)
        {
            // Resolving IScheduler instance
            var scheduler = container.Resolve<IScheduler>();

            #region Triggers

            var fiveMinuteTriggerForever = (ISimpleTrigger)TriggerBuilder.Create()
                 .WithIdentity("FiveMinuteTriggerForever", AppConstants.DefaultTaskGroup)
                 .StartNow()
                 .WithSimpleSchedule(x => x
                     .WithIntervalInMinutes(5)
                     .RepeatForever())
                 .Build();

            var twoMinuteTriggerForever = (ISimpleTrigger)TriggerBuilder.Create()
             .WithIdentity("TwoMinuteTriggerForever", AppConstants.DefaultTaskGroup)
             .StartNow()
             .WithSimpleSchedule(x => x
                 .WithIntervalInMinutes(2)
                 .RepeatForever())
             .Build();

            var fifteenSecondsTriggerForever = (ISimpleTrigger)TriggerBuilder.Create()
                    .WithIdentity("FifteenSecondsTriggerForever", AppConstants.DefaultTaskGroup)
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(15)
                        .RepeatForever())
                    .Build();

            var sixHourTriggerForever = (ISimpleTrigger)TriggerBuilder.Create()
                    .WithIdentity("SixHourTriggerForever", AppConstants.DefaultTaskGroup)
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInHours(6)
                        .RepeatForever())
                    .Build();

            /*   var twelveHourTriggerForever = (ISimpleTrigger)TriggerBuilder.Create()
                     .WithIdentity("TwelveHourTriggerForever", AppConstants.DefaultTaskGroup)
                     .StartNow()
                     .WithSimpleSchedule(x => x
                         .WithIntervalInHours(12)
                         .RepeatForever())
                     .Build();*/

            var twentyFourHourTriggerForever = (ISimpleTrigger)TriggerBuilder.Create()
                  .WithIdentity("TwentyFourHourTriggerForever", AppConstants.DefaultTaskGroup)
                  .StartNow()
                  .WithSimpleSchedule(x => x
                      .WithIntervalInHours(24)
                      .RepeatForever())
                  .Build();


    
            #endregion

            #region Send Email Job

            // Send emails every 15 seconds

            var emailJob = JobBuilder.Create<EmailJob>()
            .WithIdentity("EmailJob", AppConstants.DefaultTaskGroup)
            .Build();

            scheduler.ScheduleJob(emailJob, fifteenSecondsTriggerForever);

            #endregion

            #region Mark As Solution Job

            // Send mark as solution reminder emails

            var markAsSolutionReminderJob = JobBuilder.Create<MarkAsSolutionReminderJob>()
            .WithIdentity("MarkAsSolutionReminderJob", AppConstants.DefaultTaskGroup)
            .Build();

            scheduler.ScheduleJob(markAsSolutionReminderJob, sixHourTriggerForever);

            #endregion
        /*
            #region Ad Update Job
            var adUpdateJob = JobBuilder.Create<AdUpdateJob>()
          .WithIdentity("AdUpdateJob", AppConstants.DefaultTaskGroup)
          .Build();
            scheduler.ScheduleJob(adUpdateJob, twelveHourTriggerForever);
            #endregion
            */
            #region Report Update Job
            var reportUpdateJob = JobBuilder.Create<ReportUpdateJob>()
         .WithIdentity("ReportUpdateJob", AppConstants.DefaultTaskGroup)
         .Build();
            scheduler.ScheduleJob(reportUpdateJob, twentyFourHourTriggerForever);
            #endregion   
            // Starting scheduler
            scheduler.Start();
        }
    }
}