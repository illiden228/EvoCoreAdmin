using EvoCoreAdmin.Identity.Core.DTO;

namespace EvoCoreAdmin.Identity.Core.Abstractions;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterUserRequest request);
    Task<IReadOnlyList<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task ChangeStatusAsync(Guid id, bool isActive);
    Task AddProjectRoleAsync(Guid id, string gameProjectKey, int roleId);
    Task RemoveProjectRoleAsync(Guid id, string gameProjectKey, int roleId);
}