using EvoCoreAdmin.Identity.Core.Entities;
using EvoCoreAdmin.Identity.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvoCoreAdmin.Identity.WebHost.Seeding;

public class IdentitySeeder
{
    private readonly IdentityDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public IdentitySeeder(
        IdentityDbContext db,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        var roleNames = new[] { "Admin", "Manager", "GameDesigner", "Support" };
        var roles = new Dictionary<string, Role>();

        foreach (var roleName in roleNames)
        {
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                role = new Role { Name = roleName };
                _db.Roles.Add(role);
                await _db.SaveChangesAsync();
            }

            roles[roleName] = role;
        }

        var adminSection = _configuration.GetSection("AdminDefaults");

        var login        = adminSection["Login"]          ?? "admin";
        var password     = adminSection["Password"]       ?? "admin";
        var email        = adminSection["Email"]          ?? "admin@local";
        var roleNameConf = adminSection["Role"]           ?? "Admin";
        var projectKey   = adminSection["GameProjectKey"] ?? "core";

        if (!roles.TryGetValue(roleNameConf, out var adminRole))
        {
            adminRole = roles["Admin"];
        }

        var adminUser = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (adminUser == null)
        {
            adminUser = new User
            {
                Id       = Guid.NewGuid(),
                Login    = login,
                Email    = email,
                IsActive = true
            };

            adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, password);

            _db.Users.Add(adminUser);
            await _db.SaveChangesAsync();
        }

        bool hasRole = await _db.UserProjectRoles.AnyAsync(x =>
            x.UserId == adminUser.Id &&
            x.GameProjectKey == projectKey &&
            x.RoleId == adminRole.Id);

        if (!hasRole)
        {
            var upr = new UserProjectRole
            {
                UserId         = adminUser.Id,
                GameProjectKey = projectKey,
                RoleId         = adminRole.Id
            };

            _db.UserProjectRoles.Add(upr);
            await _db.SaveChangesAsync();
        }
    }
}
