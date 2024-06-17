using NestF.Application.Interfaces.Services;
using NestF.Infrastructure.Constants;
using Quartz;

namespace NestF.Infrastructure;

public class JobFailureListener : IJobListener
{
    public string Name => "BTSS Failure Listener";
    private readonly ITimeService _timeService;

    public JobFailureListener(ITimeService timeService)
    {
        _timeService = timeService;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        if (!context.JobDetail.JobDataMap.Contains(BackgroundConstants.RETRY_COUNT_KEY))
            context.JobDetail.JobDataMap.Put(BackgroundConstants.RETRY_COUNT_KEY, 0);
        else
        {
            var retryCount = context.JobDetail.JobDataMap.GetIntValue(BackgroundConstants.RETRY_COUNT_KEY);
            context.JobDetail.JobDataMap.Put(BackgroundConstants.RETRY_COUNT_KEY, ++retryCount);
        }
        return Task.CompletedTask;
    }

    public async Task JobWasExecuted(IJobExecutionContext context,
        JobExecutionException? jobException,
        CancellationToken cancellationToken = default)
    {
        if (jobException == null) return;
        var retryCount = context.JobDetail.JobDataMap.GetIntValue(BackgroundConstants.RETRY_COUNT_KEY);
        if (retryCount > BackgroundConstants.MAX_RETRY_COUNT)
        {
            return;
        }
        var retryAt = _timeService.Now.AddSeconds(BackgroundConstants.RETRY_SECOND_MULTYPLY * retryCount);
        var trigger = TriggerBuilder.Create()
            .WithIdentity(context.Trigger.Key)
            .StartAt(retryAt)
            .Build();
        await context.Scheduler.RescheduleJob(context.Trigger.Key, trigger, cancellationToken);
    }
}