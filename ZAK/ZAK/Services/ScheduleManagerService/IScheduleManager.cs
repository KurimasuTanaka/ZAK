using System;

namespace ZAK.Services.ScheduleManagerService;

public interface IScheduleManager
{
    public Task MoveScheduledApplicationFromOneBrigadeToAnother(int applicationId, int brigadeId, int time, int prevBrigadeId, int prevTime);
    public Task ScheduleApplication(int applicationId, int brigadeId, int time);

    public Task MakeTimeSlotEmpty(int brigadeId, int time);
}
