using Quartz;

using Unitic_BE.Abstracts;
using Unitic_BE.Constants;
using Unitic_BE.Enums;

namespace Unitic_BE.QuartzJob
{
    public class UpdateEventStatusJob : IJob
    {
        private readonly IEventRepository _repo;
        public UpdateEventStatusJob(IEventRepository repo)
        {
            _repo = repo;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // Lấy eventId từ dữ liệu truyền vào job
            var eventId = context.MergedJobDataMap.GetString("eventId");

            var myEvent = await _repo.GetEventByIdAsync(eventId);
            if (myEvent != null && myEvent.Status == EventStatus.Published)
            {
                myEvent.Status = EventStatus.InProgress;
                await _repo.UpdateEventAsync(myEvent);

                Console.WriteLine($"[Quartz Job] Event {eventId} status updated to InProgress at {DateTime.Now}");
            }
        }
    }
}
