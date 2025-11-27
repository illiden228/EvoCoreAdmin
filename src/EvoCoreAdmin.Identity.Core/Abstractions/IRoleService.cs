using EvoCoreAdmin.Identity.Core.DTO;

namespace EvoCoreAdmin.Identity.Core.Abstractions;

public interface IRoleService
{
    Task<IReadOnlyList<RoleDto>> GetAllAsync();
    Task<RoleDto?> GetByIdAsync(int id);
}