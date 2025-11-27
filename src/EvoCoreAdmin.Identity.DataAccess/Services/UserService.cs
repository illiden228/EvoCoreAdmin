using EvoCoreAdmin.Identity.Core.Abstractions;
using EvoCoreAdmin.Identity.Core.DTO;
using EvoCoreAdmin.Identity.Core.Entities;
using EvoCoreAdmin.Identity.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvoCoreAdmin.Identity.DataAccess.Services;

public class UserService : IUserService
{
    private readonly IdentityDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IdentityDbContext db, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> RegisterAsync(RegisterUserRequest request)
    {
        var existing = await _db.Users
            .FirstOrDefaultAsync(u => u.Login == request.Login || u.Email == request.Email);

        if (existing is not null)
            throw new InvalidOperationException("Пользователь с таким логином или email уже существует");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = request.Login,
            Email = request.Email,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(request.GameProjectKey))
        {
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId);
            if (role is null)
                throw new InvalidOperationException($"Роль '{request.RoleId}' не найдена");

            var upr = new UserProjectRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                GameProjectKey = request.GameProjectKey!
            };

            _db.UserProjectRoles.Add(upr);
            await _db.SaveChangesAsync();
        }

        return await MapToUserDto(user.Id);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync()
    {
        var ids = await _db.Users.Select(u => u.Id).ToListAsync();
        var result = new List<UserDto>();

        foreach (var id in ids)
        {
            result.Add(await MapToUserDto(id));
        }

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var exists = await _db.Users.AnyAsync(u => u.Id == id);
        if (!exists)
            return null;

        return await MapToUserDto(id);
    }

    public async Task ChangeStatusAsync(Guid id, bool isActive)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            throw new InvalidOperationException("Пользователь не найден");

        user.IsActive = isActive;
        await _db.SaveChangesAsync();
    }

    public async Task AddProjectRoleAsync(Guid id, string gameProjectKey, int roleId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            throw new InvalidOperationException("Пользователь не найден");

        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
        if (role is null)
            throw new InvalidOperationException($"Роль '{roleId}' не найдена");

        var exists = await _db.UserProjectRoles.AnyAsync(x =>
            x.UserId == id &&
            x.GameProjectKey == gameProjectKey &&
            x.RoleId == role.Id);

        if (exists)
            throw new InvalidOperationException("У пользователя уже есть эта роль в данном проекте");

        var upr = new UserProjectRole
        {
            UserId = id,
            RoleId = role.Id,
            GameProjectKey = gameProjectKey
        };

        _db.UserProjectRoles.Add(upr);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveProjectRoleAsync(Guid id, string gameProjectKey, int roleId)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
        if (role is null)
            throw new InvalidOperationException($"Роль '{roleId}' не найдена");

        var upr = await _db.UserProjectRoles.FirstOrDefaultAsync(x =>
            x.UserId == id &&
            x.GameProjectKey == gameProjectKey &&
            x.RoleId == role.Id);

        if (upr is null)
            throw new InvalidOperationException("Такой роли у пользователя в этом проекте нет");

        _db.UserProjectRoles.Remove(upr);
        await _db.SaveChangesAsync();
    }

    private async Task<UserDto> MapToUserDto(Guid userId)
    {
        var user = await _db.Users.FirstAsync(u => u.Id == userId);

        var projectRoles = await _db.UserProjectRoles
            .Where(x => x.UserId == userId)
            .Include(x => x.Role)
            .ToListAsync();

        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email,
            IsActive = user.IsActive,
            ProjectRoles = projectRoles
                .Select(pr => new ProjectRoleDto
                {
                    GameProjectKey = pr.GameProjectKey,
                    RoleName = pr.Role.Name
                })
                .ToList()
        };
    }
}
