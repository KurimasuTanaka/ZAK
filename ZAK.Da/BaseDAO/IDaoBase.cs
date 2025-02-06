using System;

namespace ZAK.Da.BaseDAO;

public interface IDaoBase <TransObjT, EntityT> where TransObjT : class, new() where EntityT : class
{
    Task<List<TransObjT>> GetAll();
    Task<TransObjT> GetById(int id);
    Task Insert(TransObjT entity);
    Task Update(TransObjT entity, int id);
    Task Delete(int id);
}
