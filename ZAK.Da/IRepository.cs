using System;

namespace ZAK.DA;

public interface IRepository <TEntity, TKey>
    where TEntity : class
{
    Task<TEntity> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TKey id);
}
