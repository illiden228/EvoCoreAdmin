namespace EvoCoreAdmin.Identity.Core.Entities;

public class RolePermission : BaseEntity<Guid>
{
    public int RoleId { get; set; }
    
    public Role Role { get; set; } = null!;
    
    public Guid PermissionId { get; set; }
    
    public Permission Permission { get; set; } = null!;
    
    public string? GameProjectKey { get; set; }  
}