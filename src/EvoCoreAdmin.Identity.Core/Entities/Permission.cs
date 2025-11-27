namespace EvoCoreAdmin.Identity.Core.Entities;

public class Permission : BaseEntity<Guid>
{
    public string Key { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public IEnumerable<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}