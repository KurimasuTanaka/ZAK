using System;

namespace ZAK.Services.ScheduleManagerService;

public interface IScheduleManager
{
    public Task ScheduleApplication(int applicationId, int brigadeId, int time, bool onStretching = false);

    public Task ScheduleApplicationToFirstEmptyTime(int applicationId, int brigadeId);

    public Task MakeTimeSlotEmpty(int brigadeId, int time);
}
