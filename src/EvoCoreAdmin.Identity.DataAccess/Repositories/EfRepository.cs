using EvoCoreAdmin.Identity.Core.Abstractions;
using EvoCoreAdmin.Identity.Core.Entities;
using EvoCoreAdmin.Identity.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace EvoCoreAdmin.Identity.DataAccess.Repositories;

public class EfRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : struct
{
    private readonly IdentityDbContext _db;

    public EfRepository(IdentityDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        return await _db.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity?> TryGetByIdAsync(TId id)
    {
        return await _db.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity> GetByIdAsync(TId id)
    {
        var entity = await TryGetByIdAsync(id);
        if (entity is null)
            throw new KeyNotFoundException($"Entity {typeof(TEntity).Name} with id '{id}' not found");

        return entity;
    }

    public async Task RemoveAsync(TId id)
    {
        var entity = await GetByIdAsync(id);
        _db.Set<TEntity>().Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<TId> AddAsync(TEntity entity)
    {
        await _db.Set<TEntity>().AddAsync(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _db.Set<TEntity>().AddRangeAsync(entities);
        await _db.SaveChangesAsync();
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _db.Set<TEntity>().Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}