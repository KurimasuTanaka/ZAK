using System;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZAK.Db;

namespace ZAK.Da.BaseDAO;

public class DaoBase<TransObjT, EntityT> : IDaoBase<TransObjT, EntityT>
                                                where EntityT : class
                                                where TransObjT : class, EntityT, new()
{
    private BlazorAppDbContext _dbContext;
    private ILogger<DaoBase<TransObjT, EntityT>> _logger;

    public DaoBase(BlazorAppDbContext dbContext, ILogger<DaoBase<TransObjT, EntityT>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation($"Deleting entity of type {typeof(EntityT)} with id: {id}");

        EntityT? entity = await _dbContext.Set<EntityT>().FindAsync(id);

        if (entity is null)
        {
            _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} not found");
        }
        else
        {
            _logger.LogInformation($"Entity of type {typeof(EntityT)} with id: {id} found. Deleting...");
            _dbContext.Set<EntityT>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<TransObjT>> GetAll()
    {
        _logger.LogInformation($"Getting all entities of type {typeof(EntityT)}");
        return await _dbContext.Set<EntityT>().Select(a => GenericFactory.CreateInstance<TransObjT, EntityT>(a)).ToListAsync();
    }

    public async Task<TransObjT> GetById(int id)
    {
        EntityT? entity = await _dbContext.Set<EntityT>().FindAsync(id);

        if (entity is null)
        {
            _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} not found");
            return new TransObjT();
        }
        else
        {
            _logger.LogInformation($"Entity of type {typeof(EntityT)} found. Returning");
            return GenericFactory.CreateInstance<TransObjT, EntityT>(entity);
        }
    }

    public async Task Insert(TransObjT entity)
    {
        _logger.LogInformation($"Inserting new entity of type {typeof(EntityT)}");
        await _dbContext.Set<EntityT>().AddAsync(entity);
    }

    public async Task Update(TransObjT entity, int id)
    {
        EntityT? oldEntity = await _dbContext.Set<EntityT>().FindAsync(id);

        if (oldEntity is null)
        {
            _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} not found. Adding new entity...");
            await _dbContext.Set<EntityT>().AddAsync(entity);
        }
        else
        {
            _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} found. Updating...");

            _dbContext.Set<EntityT>().Entry(_dbContext.Set<EntityT>().FindAsync(id).Result!).CurrentValues.SetValues(entity);
            await _dbContext.SaveChangesAsync();
        }

    }
}
