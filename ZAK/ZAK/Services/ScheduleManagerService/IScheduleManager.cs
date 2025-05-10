using System;

namespace ZAK.Services.ScheduleManagerService;

public interface IScheduleManager
{
    public Task InsertApplication(int applicationId, int brigadeId, int time, int prevBrigadeId, int prevTime);
    public Task InsertNewApplicationInEmptySlot(int applicationId, int brigadeId, int time);

    public Task MakeSlotEmpty(int brigadeId, int time);
}
