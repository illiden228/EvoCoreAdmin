using EvoCoreAdmin.Identity.Core.DTO;

namespace EvoCoreAdmin.Identity.Core.Abstractions;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
}