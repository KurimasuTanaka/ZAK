using ZAK.Db.Models;

namespace BlazorApp.DA;

public interface IBrigadesDataAccess
{
    public Task AddNewBrigade();
    public Task DeleteBrigade(int id);
    public Task ChangeBrigadeNumber(int id, int number);
    public Task ChangeBrigadeApplication(int id, int time, int applicationId);
    public Task DeleteApplicationFromSchedule(int brigadeId, int applicationId);
    public Task<List<Brigade>> GetAllBrigades();
    public Task SwapApplications(int brigade1Id, int brigade2Id, int application1Time, int application2Time);
}
