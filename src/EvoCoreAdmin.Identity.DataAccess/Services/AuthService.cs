using EvoCoreAdmin.Identity.Core.Abstractions;
using EvoCoreAdmin.Identity.Core.DTO;
using EvoCoreAdmin.Identity.Core.Entities;
using EvoCoreAdmin.Identity.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvoCoreAdmin.Identity.DataAccess.Services;

public class AuthService : IAuthService
{
    private readonly IdentityDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        IdentityDbContext db,
        IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .Include(u => u.ProjectRoles)
                .ThenInclude(pr => pr.Role)
            .FirstOrDefaultAsync(u => u.Login == request.Login);

        if (user is null || !user.IsActive)
            throw new InvalidOperationException("Неверный логин или пароль");

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
            throw new InvalidOperationException("Неверный логин или пароль");

        var roleNames = user.ProjectRoles
            .Select(pr => pr.Role.Name)
            .Distinct()
            .ToList();

        return new AuthResult
        {
            UserId = user.Id,
            Login = user.Login,
            Roles = roleNames
        };
    }

    public async Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null || !user.IsActive)
            throw new InvalidOperationException("Пользователь не найден или не активен");

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
        if (verifyResult == PasswordVerificationResult.Failed)
            throw new InvalidOperationException("Старый пароль неверен");

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        await _db.SaveChangesAsync();
    }
}
