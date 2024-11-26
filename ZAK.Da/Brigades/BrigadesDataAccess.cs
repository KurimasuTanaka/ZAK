using ZAK.Db;
using ZAK.Db.Models;
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

    public async Task DeleteApplicationFromSchedule(int brigadeId, int applicationId)
    {
        BrigadeModel? brigade = await _dbContext.brigades.FindAsync(brigadeId);
        if (brigade is not null)
        {
            brigade.applicationsIds[brigade.applicationsIds.IndexOf(applicationId)] = 0;
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


    public async Task SwapApplications(int brigade1Id, int brigade2Id, int application1Time, int application2Time)
    {

        if (brigade1Id == brigade2Id)
        {
            BrigadeModel? brigade = await _dbContext.brigades.FindAsync(brigade1Id);
            if (brigade is not null)
            {
                int applicationIdBuf = brigade.applicationsIds[application1Time];
                brigade.applicationsIds[application1Time] = brigade.applicationsIds[application2Time];
                brigade.applicationsIds[application2Time] = applicationIdBuf;

                await _dbContext.SaveChangesAsync();
            }
        }
        else
        {
            BrigadeModel? brigade1 = await _dbContext.brigades.FindAsync(brigade1Id);
            BrigadeModel? brigade2 = await _dbContext.brigades.FindAsync(brigade2Id);

            if (brigade1 is not null && brigade2 is not null)
            {
                int applicationIdBuf = brigade1.applicationsIds[application1Time];
                brigade1.applicationsIds[application1Time] = brigade2.applicationsIds[application2Time];
                brigade2.applicationsIds[application2Time] = applicationIdBuf;

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
