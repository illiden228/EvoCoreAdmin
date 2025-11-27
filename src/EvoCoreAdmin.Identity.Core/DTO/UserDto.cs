namespace EvoCoreAdmin.Identity.Core.DTO;

public class UserDto
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; }

    public List<ProjectRoleDto> ProjectRoles { get; set; } = new();
}