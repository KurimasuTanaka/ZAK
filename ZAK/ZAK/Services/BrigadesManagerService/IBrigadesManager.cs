using System;

namespace ZAK.Services.BrigadesManagerService;

public interface IBrigadesManager
{
    public Task InsertApplication(int applicationId, int brigadeId, int time, int prevBrigadeId, int prevTime);
    public Task InsertNewApplicationInEmptySlot(int applicationId, int brigadeId, int time);
}
