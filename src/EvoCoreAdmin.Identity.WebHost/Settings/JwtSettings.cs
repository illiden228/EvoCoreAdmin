namespace EvoCoreAdmin.Identity.WebHost.Settings;

public class JwtSettings
{
    public string SecretKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int LifetimeMinutes { get; set; } = 60;
}