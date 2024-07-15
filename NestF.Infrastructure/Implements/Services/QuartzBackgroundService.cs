using NestF.Application.Interfaces.Services;
using NestF.Infrastructure.Constants;
using NestF.Infrastructure.Jobs;
using Quartz;

namespace NestF.Infrastructure.Implements.Services;

public class QuartzBackgroundService : IBackgroundService
{
    private readonly ITimeService _timeService;
    private readonly IScheduler _scheduler;
    public QuartzBackgroundService(ITimeService timeService, ISchedulerFactory factory)
    {
        _timeService = timeService;
        _scheduler = factory.GetScheduler().Result;
    }

    public async Task EnqueueSendPasswordMailJobAsync(int accountId)
    {
        var key = $"{nameof(SendPasswordMailJob)}_{accountId}";
        var jobKey = new JobKey(key,BackgroundConstants.ACCOUNT_GRP);
        var triggerKey = new TriggerKey(key, BackgroundConstants.ACCOUNT_GRP);
        var jobDetail = JobBuilder.Create<SendPasswordMailJob>()
            .WithIdentity(jobKey)
            .UsingJobData(BackgroundConstants.ACCOUNT_ID_KEY, accountId)
            .RequestRecovery()
            .PersistJobDataAfterExecution()
            .Build();
        // Define the trigger
        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .StartNow()
            .Build();
        // Schedule the job with the trigger
        await _scheduler.ScheduleJob(jobDetail, trigger);
    }
}