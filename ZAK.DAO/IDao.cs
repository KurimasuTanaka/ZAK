using System;
using Microsoft.EntityFrameworkCore;

namespace ZAK.DAO;

public interface IDao<TransObjT, EntityT> where TransObjT : class, new() where EntityT : class
{
    Task<IEnumerable<TransObjT>> GetAll(Func<IQueryable<EntityT>, IQueryable<EntityT>>? query = null);
    Task<TransObjT> GetById(int id);
    Task Insert(TransObjT entity,  Func<IQueryable<EntityT>, TransObjT, DbContext, EntityT>? inputProcessQuery = null);
    Task InsertRange(IEnumerable<EntityT> entities, Func<IEnumerable<EntityT>, DbContext, IEnumerable<EntityT>>? inputProcessQuery = null);
    Task Update(TransObjT entity,
        Func<EntityT, bool> findPredicate,
        Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null);
    Task Update(TransObjT entity, int id,
    Func<EntityT, bool> findPredicate,
    Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null,
    Func<TransObjT, DbContext, TransObjT>? inputDataProccessingQuery = null
    );

    Task UpdateRange(
        IEnumerable<TransObjT> entities, 
        Func<EntityT, bool> findPredicate, 
        Func<IEnumerable<EntityT>, EntityT, EntityT> enitySeach,
        Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null,
        Func<DbContext, EntityT, EntityT>? attachFunction = null,
        Func<EntityT, EntityT, EntityT>? updatingFunction = null); 

    Task UpdateRange(IEnumerable<TransObjT> entities);
    
    Task Delete(int id);
    Task DeleteAll();
    Task DeleteRange(IEnumerable<TransObjT> entities);
}
