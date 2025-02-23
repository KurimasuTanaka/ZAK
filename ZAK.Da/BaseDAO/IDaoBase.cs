using System;
using Microsoft.EntityFrameworkCore;
using ZAK.Db;

namespace ZAK.Da.BaseDAO;

public interface IDaoBase<TransObjT, EntityT> where TransObjT : class, new() where EntityT : class
{
    Task<IEnumerable<TransObjT>> GetAll(Func<IQueryable<EntityT>, IQueryable<EntityT>>? query = null);
    Task<TransObjT> GetById(int id);
    Task Insert(TransObjT entity);
    Task InsertRange(IEnumerable<TransObjT> entities);
    Task Update(TransObjT entity, int id,
        Func<EntityT, bool>? findPredicate,
        Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null);
    Task Update(TransObjT entity, int id,
    Func<EntityT, bool>? findPredicate,
    Func<IQueryable<EntityT>, IQueryable<EntityT>>? includeQuery = null,
    Func<TransObjT, DbContext, TransObjT>? inputDataProccessingQuery = null
    );

    Task Update(TransObjT entity, int id);


    Task Delete(int id);
    Task DeleteAll();
}
