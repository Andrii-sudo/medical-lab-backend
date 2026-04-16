using Hangfire;
using LabAPI.Services;

namespace LabAPI.BackgroundJobs;
public static class RecurringJobs
{
    public static void Register()
    {
        RecurringJob.AddOrUpdate<ISampleService>(
            "update-expired-samples",
            service => service.UpdateExpiredSamples(),
            Cron.Daily);

        RecurringJob.TriggerJob("update-expired-samples");
    }
}
