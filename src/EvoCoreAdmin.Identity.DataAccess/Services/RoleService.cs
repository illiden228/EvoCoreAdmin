using EvoCoreAdmin.Identity.Core.Abstractions;
using EvoCoreAdmin.Identity.Core.DTO;
using EvoCoreAdmin.Identity.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace EvoCoreAdmin.Identity.DataAccess.Services;

public class RoleService : IRoleService
{
    private readonly IdentityDbContext _db;

    public RoleService(IdentityDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllAsync()
    {
        return await _db.Roles
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync();
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        return await _db.Roles
            .Where(r => r.Id == id)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .FirstOrDefaultAsync();
    }
}