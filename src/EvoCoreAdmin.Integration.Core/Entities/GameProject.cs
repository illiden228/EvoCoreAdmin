namespace EvoCoreAdmin.Integration.Core.Entities;

public class GameProject : BaseEntity
{
    public string Key { get; set; } = null!; 
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public string ApiBaseUrl { get; set; } = null!;
    public string ApiKey { get; set; } = null!;

    public GameConnectorType ConnectorType { get; set; }
}