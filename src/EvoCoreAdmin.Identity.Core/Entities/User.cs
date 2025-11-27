namespace EvoCoreAdmin.Identity.Core.Entities;

public class User : BaseEntity<Guid>
{
    public string Email { get; set; } = null!;
    public string Login { get; set; } = null!;
    
    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public IEnumerable<UserProjectRole> ProjectRoles { get; set; } = new List<UserProjectRole>();
}