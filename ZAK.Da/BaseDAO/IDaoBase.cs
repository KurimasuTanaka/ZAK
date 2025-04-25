using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ZAK.Db;

namespace ZAK.Da.BaseDAO;

public interface IDaoBase<TransObjT, EntityT> where TransObjT : class, new() where EntityT : class
{
    Task<IEnumerable<TransObjT>> GetAll(Func<IQueryable<EntityT>, IQueryable<EntityT>>? query = null);
    Task<TransObjT> GetById(int id);
    Task Insert(TransObjT entity,  Func<IQueryable<EntityT>, TransObjT, DbContext, EntityT>? inputProcessQuery = null);
    Task InsertRange(IEnumerable<TransObjT> entities, Func<IQueryable<EntityT>, DbContext, IQueryable<EntityT>>? inputProcessQuery = null);
    Task Update(TransObjT entity, int id,
        Func<EntityT, bool>? findPredicate,
        Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null);
    Task Update(TransObjT entity, int id,
    Func<EntityT, bool>? findPredicate = null,
    Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null,
    Func<TransObjT, DbContext, TransObjT>? inputDataProccessingQuery = null
    );

    Task Update(TransObjT entity, int id);

    Task UpdateRange(
        IEnumerable<TransObjT> entities, 
        Func<EntityT, bool> findPredicate, 
        Func<IEnumerable<EntityT>, EntityT, EntityT> enitySeach,
        Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null,
        Func<DbContext, EntityT, EntityT>? attachFunction = null,
        Func<EntityT, EntityT, EntityT>? updatingFunction = null); 

    Task Delete(int id);
    Task DeleteAll();
    Task DeleteRange(IEnumerable<TransObjT> entities);
}
