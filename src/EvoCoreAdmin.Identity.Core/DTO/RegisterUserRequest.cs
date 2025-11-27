namespace EvoCoreAdmin.Identity.Core.DTO;

public class RegisterUserRequest
{
    public string Login { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? GameProjectKey { get; set; }
    public int RoleId { get; set; }
}