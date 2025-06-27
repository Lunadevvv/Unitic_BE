using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Abstracts;
using Unitic_BE.QuartzJob;

namespace Unitic_BE.QuartzScheduler
{
    public class QuartzEventJobScheduler : IEventJobScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public QuartzEventJobScheduler(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task ScheduleUpdateStatusJobAsync(string eventId, DateTime? startDate)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var jobKey = new JobKey($"UpdateEventStatusJob_{eventId}");

            //if (await scheduler.CheckExists(jobKey))
            //    await scheduler.DeleteJob(jobKey);

            var job = JobBuilder.Create<UpdateEventStatusJob>()
                .WithIdentity(jobKey)
                .UsingJobData("eventId", eventId)
                .Build();
            //mặc định non durable (ko giữ lại job nếu ko còn trigger)

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"Trigger_{eventId}")
                .StartAt(startDate.Value)
                .ForJob(jobKey)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public async Task DeleteStatusJobAsync(string eventId)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey($"UpdateEventStatusJob_{eventId}");

            if (await scheduler.CheckExists(jobKey))
                await scheduler.DeleteJob(jobKey);
        }
    }

}
