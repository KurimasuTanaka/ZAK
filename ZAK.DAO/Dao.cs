using System;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DAO;

public class Dao<TransObjT, EntityT> : IDao<TransObjT, EntityT>
                                                where EntityT : class
                                                where TransObjT : class, EntityT, new()
{
    private IDbContextFactory<BlazorAppDbContext> _dbContextFactory;
    private ILogger<Dao<TransObjT, EntityT>> _logger;

    public Dao(IDbContextFactory<BlazorAppDbContext> dbContextFactory, ILogger<Dao<TransObjT, EntityT>> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation($"Deleting entity of type {typeof(EntityT)} with id: {id}");

        await using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
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

        await using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            dbContext.Set<EntityT>().RemoveRange(dbContext.Set<EntityT>());
            await dbContext.SaveChangesAsync();
        }

        _logger.LogInformation("All entities deleted successfully!");
    }

    public async Task DeleteRange(IEnumerable<TransObjT> entities)
    {
        _logger.LogInformation($"Deleting range of entities of type {typeof(EntityT)}");

        await using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            dbContext.Set<EntityT>().RemoveRange(entities.Cast<EntityT>());
            await dbContext.SaveChangesAsync();
        }
        _logger.LogInformation($"Entities deleted successfully");
    }

    public async Task<IEnumerable<TransObjT>> GetAll(Func<IQueryable<EntityT>, IQueryable<EntityT>>? query = null)
    {
        _logger.LogInformation($"Getting all entities of type {typeof(EntityT)}");

        await using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            if (query is not null) return await query(dbContext.Set<EntityT>()).Select(a => GenericFactory.CreateInstance<TransObjT, EntityT>(a)).ToListAsync();
            else return await dbContext.Set<EntityT>().Select(a => GenericFactory.CreateInstance<TransObjT, EntityT>(a)).ToListAsync();
        }
    }

    public async Task<TransObjT> GetById(int id)
    {
        await using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
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
        await using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            EntityT insertedEntity = null;
            if (inputProcessQuery is not null) insertedEntity = inputProcessQuery(dbContext.Set<EntityT>(), entity, dbContext);
            else insertedEntity = entity;

            await dbContext.Set<EntityT>().AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task InsertRange(IEnumerable<EntityT> entities, Func<IEnumerable<EntityT>, DbContext, IEnumerable<EntityT>>? inputProcessQuery = null)
    {
        _logger.LogInformation($"Inserting range of entities of type {typeof(EntityT)}");


        await using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            if (inputProcessQuery is not null) entities = inputProcessQuery(entities, dbContext);

            dbContext.Set<EntityT>().AddRange(entities.Cast<EntityT>());
            string stack = Environment.StackTrace;

            try
            {


                int amount = dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                int i = 0;
            }
        }
    }

    public async Task Update(
        TransObjT entity,
        Func<EntityT, bool> findPredicate,
        Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null)
    {
        await using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            IQueryable<EntityT> baseQuery = dbContext.Set<EntityT>();

            if (includeQuery is not null) baseQuery = includeQuery(baseQuery);

            EntityT? oldEntity = baseQuery.FirstOrDefault(findPredicate);

            if (oldEntity is null)
            {
                _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {GetPrimaryKeyValue<EntityT>(dbContext, entity)} not found. Adding new entity...");
                await dbContext.Set<EntityT>().AddAsync(entity);
            }
            else
            {
                _logger.LogWarning($"Entity of type {typeof(EntityT)} with id: {GetPrimaryKeyValue<EntityT>(dbContext, entity)} found. Updating...");

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
                if (findPredicate is null) throw new Exception("Find predicate should not be null if inlcude query is used!");

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
        Func<EntityT, bool> findPredicate,
        Func<IEnumerable<EntityT>, EntityT, EntityT> entitySeach,
        Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null,
        Func<DbContext, EntityT, EntityT>? attachFunction = null,
        Func<EntityT, EntityT, EntityT>? updatingFunction = null)
    {
        _logger.LogInformation($"Updating range of entities of type {typeof(EntityT)}");

        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {
            IQueryable<EntityT> baseQuery = dbContext.Set<EntityT>().AsTracking();

            if (includeQuery is not null) baseQuery = includeQuery(baseQuery);
            List<EntityT> oldEntities = baseQuery.AsEnumerable().Where(a => findPredicate(a)).ToList();

            for (int i = 0; i < oldEntities.Count(); i++)
            {
                if (updatingFunction is null)
                {
                    foreach (PropertyInfo property in typeof(EntityT).GetProperties().Where(p => p.CanWrite))
                    {
                        property.SetValue(oldEntities[i], property.GetValue(entitySeach(entities, oldEntities[i]), null), null);
                    }
                }
                else
                {
                    oldEntities[i] = updatingFunction(oldEntities[i], entitySeach(entities, oldEntities[i]));
                }
                if (attachFunction is not null) oldEntities[i] = attachFunction(dbContext, oldEntities[i]);

            }
            await dbContext.SaveChangesAsync();
        }


        _logger.LogInformation($"Entities updated successfully");
    }


    async Task IDao<TransObjT, EntityT>.UpdateRange(IEnumerable<TransObjT> entities)
    {
        using (BlazorAppDbContext dbContext = _dbContextFactory.CreateDbContext())
        {

            dbContext.UpdateRange(entities);
            await dbContext.SaveChangesAsync();
        }
    }

    private object? GetPrimaryKeyValue<TEntity>(DbContext context, TEntity entity)
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity));
        var key = entityType?.FindPrimaryKey();

        if (key == null)
            return null;

        if (key.Properties.Count == 1)
        {
            // Single key
            return key.Properties[0].PropertyInfo?.GetValue(entity);
        }

        // Composite key
        var keyValues = new Dictionary<string, object?>();
        foreach (var prop in key.Properties)
        {
            keyValues[prop.Name] = prop.PropertyInfo?.GetValue(entity);
        }
        return keyValues;
    }
}

