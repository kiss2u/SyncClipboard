namespace SyncClipboard.Core.Models.UserConfigs;

public record class EnvConfig
{
    public bool PortableUserConfig { get; set; } = false;

    public bool PortableAppDataFolder { get; set; } = false;
}
