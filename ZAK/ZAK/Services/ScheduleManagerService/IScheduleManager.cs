using System;

namespace ZAK.Services.ScheduleManagerService;

public interface IScheduleManager
{
    public Task MoveScheduledApplicationFromOneBrigadeToAnother(int applicationId, int brigadeId, int newTime, int prevBrigadeId, int prevTime);
    public Task MoveScheduledApplicationFromOneTimeToAnother(int applicationId, int brigadeId, int newTime, int prevTime);
    public Task ScheduleApplication(int applicationId, int brigadeId, int time);

    public Task MakeTimeSlotEmpty(int brigadeId, int time);
}
