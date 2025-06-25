using System;

namespace ZAK.Services.ScheduleManagerService;

public interface IScheduleManager
{
    public Task ScheduleApplication(int applicationId, int brigadeId, int time);

    public Task ScheduleApplicationToFirstEmptyTime(int applicationId, int brigadeId);

    public Task MakeTimeSlotEmpty(int brigadeId, int time);
}
