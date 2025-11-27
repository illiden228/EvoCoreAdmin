namespace EvoCoreAdmin.Identity.Core.Entities;

public class UserProjectRole : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public string? GameProjectKey { get; set; }
}