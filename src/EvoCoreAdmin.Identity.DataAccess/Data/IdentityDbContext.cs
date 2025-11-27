using EvoCoreAdmin.Identity.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvoCoreAdmin.Identity.DataAccess.Data;

public class IdentityDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<UserProjectRole> UserProjectRoles { get; set; } = null!;

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigurePermission(modelBuilder);
        ConfigureRolePermission(modelBuilder);
        ConfigureUserProjectRole(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
    
    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();

        user.Property(u => u.Login).IsRequired().HasMaxLength(50);
        user.Property(u => u.Email).IsRequired().HasMaxLength(100);
        user.Property(u => u.PasswordHash).IsRequired().HasMaxLength(200);
        user.Property(u => u.IsActive).HasDefaultValue(true);
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        var role = modelBuilder.Entity<Role>();

        role.Property(r => r.Name).IsRequired().HasMaxLength(50);
        role.HasIndex(r => r.Name).IsUnique();
    }

    private static void ConfigurePermission(ModelBuilder modelBuilder)
    {
        var permission = modelBuilder.Entity<Permission>();

        permission.Property(p => p.Key).IsRequired().HasMaxLength(100);
        permission.HasIndex(p => p.Key).IsUnique();

        permission.Property(p => p.Description).HasMaxLength(250);
    }

    private static void ConfigureRolePermission(ModelBuilder modelBuilder)
    {
        var rp = modelBuilder.Entity<RolePermission>();

        rp.Property(x => x.GameProjectKey).HasMaxLength(50);

        rp.HasOne(x => x.Role)
          .WithMany(r => r.RolePermissions)
          .HasForeignKey(x => x.RoleId)
          .OnDelete(DeleteBehavior.Cascade);

        rp.HasOne(x => x.Permission)
          .WithMany(p => p.RolePermissions)
          .HasForeignKey(x => x.PermissionId)
          .OnDelete(DeleteBehavior.Cascade);

        rp.HasIndex(x => new { x.RoleId, x.PermissionId, x.GameProjectKey }).IsUnique();
    }

    private static void ConfigureUserProjectRole(ModelBuilder modelBuilder)
    {
        var upr = modelBuilder.Entity<UserProjectRole>();

        upr.Property(x => x.GameProjectKey)
           .IsRequired()
           .HasMaxLength(50);

        upr.HasOne(x => x.User)
           .WithMany(u => u.ProjectRoles)
           .HasForeignKey(x => x.UserId)
           .OnDelete(DeleteBehavior.Cascade);

        upr.HasOne(x => x.Role)
           .WithMany(r => r.ProjectRoles)
           .HasForeignKey(x => x.RoleId)
           .OnDelete(DeleteBehavior.Cascade);

        upr.HasIndex(x => new { x.UserId, x.GameProjectKey, x.RoleId }).IsUnique();
    }
}