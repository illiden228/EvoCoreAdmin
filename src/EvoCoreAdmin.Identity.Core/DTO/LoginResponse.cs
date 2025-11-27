namespace EvoCoreAdmin.Identity.Core.DTO;

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}