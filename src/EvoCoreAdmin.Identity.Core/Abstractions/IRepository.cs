using EvoCoreAdmin.Identity.Core.Entities;

namespace EvoCoreAdmin.Identity.Core.Abstractions;

public interface IRepository<TEntity, TId> 
    where TEntity : BaseEntity<TId>
    where TId : struct
{
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(TId id);
    Task<TEntity?> TryGetByIdAsync(TId id);
    Task RemoveAsync(TId id);

    Task<TId> AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    Task<TEntity> UpdateAsync(TEntity entity);
}