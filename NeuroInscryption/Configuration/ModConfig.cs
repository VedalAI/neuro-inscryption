using BepInEx.Configuration;

namespace NeuroInscryption.Configuration;

public sealed partial class ModConfig(ConfigFile file)
{
    public ConfigEntry<string> WebsocketUrl { get; } = file.Bind("Configuration", "WebsocketUrl", "ws://localhost:8000");
}
