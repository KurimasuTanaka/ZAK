using System;
using Microsoft.EntityFrameworkCore;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class CoefficientRepository : ICoefficientRepository
{
    private readonly IDbContextFactory<ZakDbContext> _dbContextFactory;

    public CoefficientRepository(IDbContextFactory<ZakDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public Task CreateAsync(Coefficient entity)
    {
        throw new Exception("Method should not be called");
    }

    public Task DeleteAsync(int id)
    {
        throw new Exception("Method should not be called");
    }

    public async Task<IEnumerable<Coefficient>> GetAllAsync()
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            return await context.coefficients
                .Select(c => new Coefficient(c))
                .ToListAsync();
        }
    }

    public async Task<Coefficient> GetByIdAsync(int id)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            CoefficientModel? coefficientModel = await context.coefficients.Where(c => c.id == id).FirstOrDefaultAsync();
            if (coefficientModel is null) throw new Exception($"Coefficient with id {id} not found");
            return new Coefficient(coefficientModel);
        }
    }

    public async Task UpdateAsync(Coefficient entity)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            CoefficientModel? coefficientModel = await context.coefficients.Where(c => c.id == entity.id).FirstOrDefaultAsync();
            if(coefficientModel is not null) coefficientModel.coefficient = entity.coefficient;
            await context.SaveChangesAsync();
        }
    }
}
