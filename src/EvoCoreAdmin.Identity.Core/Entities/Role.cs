namespace EvoCoreAdmin.Identity.Core.Entities;

public class Role: BaseEntity<int>
{
    public string Name { get; set; } = null!;

    public IEnumerable<UserProjectRole> ProjectRoles { get; set; } = new List<UserProjectRole>();
    public IEnumerable<RolePermission> RolePermissions { get; set; } = new List<RolePermission>(); 
}