using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.Da.BaseDAO;

public class DaoBase<TransObjT, EntityT> : IDaoBase<TransObjT, EntityT>
                                                where EntityT : class
                                                where TransObjT : class, EntityT, new()
{
    private IDbContextFactory<BlazorAppDbContext> _dbContextFactory;
    private ILogger<DaoBase<TransObjT, EntityT>> _logger;

    public DaoBase(IDbContextFactory<BlazorAppDbContext> dbContextFactory, ILogger<DaoBase<TransObjT, EntityT>> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation($"Deleting entity of type {typeof(EntityT)} with id: {id}");

        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {

            EntityT? entity = await dbContext.Set<EntityT>().FindAsync(id);

            if (entity is null)
            {
                _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} not found");
            }
            else
            {
                _logger.LogInformation($"Entity of type {typeof(EntityT)} with id: {id} found. Deleting...");
                dbContext.Set<EntityT>().Remove(entity);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    public async Task DeleteAll()
    {
        _logger.LogInformation($"Deleting all entities of type {typeof(EntityT)}");

        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            dbContext.Set<EntityT>().RemoveRange(dbContext.Set<EntityT>());
            await dbContext.SaveChangesAsync();
        }

        _logger.LogInformation("All entities deleted successfully!");
    }

    public async Task DeleteRange(IEnumerable<TransObjT> entities)
    {
        _logger.LogInformation($"Deleting range of entities of type {typeof(EntityT)}");

        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            dbContext.Set<EntityT>().RemoveRange(entities.Cast<EntityT>());
            await dbContext.SaveChangesAsync();
        }
        _logger.LogInformation($"Entities deleted successfully");
    }

    public async Task<IEnumerable<TransObjT>> GetAll(Func<IQueryable<EntityT>, IQueryable<EntityT>>? query = null)
    {
        _logger.LogInformation($"Getting all entities of type {typeof(EntityT)}");

        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            if (query is not null) return await query(dbContext.Set<EntityT>()).Select(a => GenericFactory.CreateInstance<TransObjT, EntityT>(a)).ToListAsync();
            else return await dbContext.Set<EntityT>().Select(a => GenericFactory.CreateInstance<TransObjT, EntityT>(a)).ToListAsync();
        }
    }

    public async Task<TransObjT> GetById(int id)
    {
        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            EntityT? entity = await dbContext.Set<EntityT>().FindAsync(id);
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
    }

    public async Task Insert(TransObjT entity, Func<IQueryable<EntityT>, TransObjT, DbContext, EntityT>? inputProcessQuery = null)
    {
        _logger.LogInformation($"Inserting new entity of type {typeof(EntityT)}");


        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            EntityT insertedEntity = null;
            if(inputProcessQuery is not null) insertedEntity = inputProcessQuery(dbContext.Set<EntityT>(), entity, dbContext);
            else insertedEntity = entity;
            
            await dbContext.Set<EntityT>().AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task InsertRange(IEnumerable<TransObjT> entities, Func<IQueryable<EntityT>, DbContext, IQueryable<EntityT>>? inputProcessQuery = null)
    {
        _logger.LogInformation($"Inserting range of entities of type {typeof(EntityT)}");

        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            await dbContext.Set<EntityT>().AddRangeAsync(entities.Cast<EntityT>());
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task Update(
        TransObjT entity, int id,
        Func<EntityT, bool>? findPredicate,
        Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null
        )
    {

        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {


            IQueryable<EntityT> baseQuery = dbContext.Set<EntityT>();

            if (includeQuery is not null) baseQuery = includeQuery(baseQuery);

            EntityT? oldEntity = baseQuery.FirstOrDefault(findPredicate);

            if (oldEntity is null)
            {
                _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} not found. Adding new entity...");
                await dbContext.Set<EntityT>().AddAsync(entity);
            }
            else
            {
                _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} found. Updating...");

                foreach (PropertyInfo property in typeof(EntityT).GetProperties().Where(p => p.CanWrite))
                {
                    property.SetValue(oldEntity, property.GetValue(entity, null), null);
                }

                await dbContext.SaveChangesAsync();
            }

        }
    }

    public async Task Update(TransObjT entity, int id)
    {

        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {

            EntityT? oldEntity = await dbContext.Set<EntityT>().FindAsync(id);

            if (oldEntity is null)
            {
                _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} not found. Adding new entity...");
                await dbContext.Set<EntityT>().AddAsync(entity);
            }
            else
            {
                _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} found. Updating...");

                foreach (PropertyInfo property in typeof(EntityT).GetProperties().Where(p => p.CanWrite))
                {
                    property.SetValue(oldEntity, property.GetValue(entity, null), null);
                }
            }
            await dbContext.SaveChangesAsync();

        }
    }

    public async Task Update(
        TransObjT entity,
        int id,
        Func<EntityT, bool>? findPredicate = null,
        Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null,
        Func<TransObjT, DbContext, TransObjT>? inputDataProccessingQuery = null)
    {
        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            entity = inputDataProccessingQuery is not null ? inputDataProccessingQuery(entity, dbContext) : entity;

            IQueryable<EntityT> baseQuery = dbContext.Set<EntityT>().AsTracking();


            EntityT? oldEntity = null;
            if (includeQuery is not null)
            {
                if(findPredicate is null) throw new Exception("Find predicate should not be null if inlcude query is used!");

                baseQuery = includeQuery(baseQuery);
                oldEntity = baseQuery.FirstOrDefault(findPredicate);
            }
            else oldEntity = await dbContext.Set<EntityT>().FindAsync(id);


            if (oldEntity is null)
            {
                _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} not found. Adding new entity...");
                await dbContext.Set<EntityT>().AddAsync(entity);
            }
            else
            {
                _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {id} found. Updating...");

                foreach (PropertyInfo property in typeof(EntityT).GetProperties().Where(p => p.CanWrite))
                {
                    property.SetValue(oldEntity, property.GetValue(entity, null), null);
                }

                dbContext.Update(oldEntity);
                await dbContext.SaveChangesAsync();
            }

        }
    }

    public async Task UpdateRange(
        IEnumerable<TransObjT> entities, 
        Func<EntityT, bool> findPredicate)
    {  
        _logger.LogInformation($"Updating range of entities of type {typeof(EntityT)}");

        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            List<EntityT> oldEntities = new();
            
            foreach (EntityT entity in entities)
            {
                EntityT? oldEntity = dbContext.Set<EntityT>().FirstOrDefault(findPredicate);
                if(oldEntity is not null)
                {
                    foreach (PropertyInfo property in typeof(EntityT).GetProperties().Where(p => p.CanWrite))
                    {
                        property.SetValue(oldEntity, property.GetValue(entity, null), null);
                    }
                    oldEntities.Append(oldEntity);
                }
            }


            dbContext.UpdateRange(oldEntities.Cast<EntityT>());
            await dbContext.SaveChangesAsync();
        }


        _logger.LogInformation($"Entities updated successfully");
    }
}

