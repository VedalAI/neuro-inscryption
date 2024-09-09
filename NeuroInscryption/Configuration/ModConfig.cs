using BepInEx.Configuration;

namespace NeuroInscryption.Configuration;

internal sealed class ModConfig(ConfigFile file)
{
    public ConfigEntry<string> WebsocketUrl { get; } = file.Bind("Configuration", "WebsocketUrl", "ws://localhost:8000");

    public ConfigEntry<float> MouseSpeedMultiplier { get; } = file.Bind("Configuration", "MouseSpeedMultiplier", 1f);

    public ConfigEntry<ChallengeFlags> Challenges { get; } = file.Bind("Configuration", "Challenges", ChallengeFlags.None);
}
