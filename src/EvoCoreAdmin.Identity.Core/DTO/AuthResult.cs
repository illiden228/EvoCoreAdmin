namespace EvoCoreAdmin.Identity.Core.DTO;

public class AuthResult
{
    public Guid UserId { get; set; }
    public string Login { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}