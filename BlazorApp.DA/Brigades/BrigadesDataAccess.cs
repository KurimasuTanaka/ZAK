using BlazorApp.DB;
using BlazorApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.DA;

public class BrigadesDataAccess : IBrigadesDataAccess
{

    private readonly BlazorAppDbContext _dbContext;

    public BrigadesDataAccess(BlazorAppDbContext blazorAppDbContext) => _dbContext = blazorAppDbContext;

    public async Task AddNewBrigade()
    {
        await _dbContext.brigades.AddAsync(new BrigadeModel());
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task ChangeBrigadeApplication(int id, int time, int applicationId)
    {
        BrigadeModel? brigade = await _dbContext.brigades.FindAsync(id);

        if (brigade is not null)
        {
            brigade.applicationsIds[time] = applicationId;
            await _dbContext.SaveChangesAsync();
        }

        return;
    }

    public async Task ChangeBrigadeNumber(int id, int number)
    {
        BrigadeModel? brigade = await _dbContext.brigades.FindAsync(id);
        if (brigade is not null)
        {
            brigade.brigadeNumber = number;

            await _dbContext.SaveChangesAsync();
        }
        return;
    }

    public async Task DeleteBrigade(int id)
    {
        BrigadeModel? brigade = await _dbContext.brigades.FindAsync(id);
        if (brigade is not null)
        {
            _dbContext.brigades.Remove(brigade);

            _dbContext.SaveChanges();
        }
        return;
    }

    public async Task<List<Brigade>> GetAllBrigades()
    {
        return await _dbContext.brigades.Select(brigade => new Brigade(brigade)).ToListAsync();
    }

    public Task SwapApplications()
    {
        throw new NotImplementedException();
    }
}
