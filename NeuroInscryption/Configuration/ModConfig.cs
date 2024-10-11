using BepInEx.Configuration;

namespace NeuroInscryption.Configuration;

public sealed class ModConfig(ConfigFile file)
{
    private const string Section = "Configuration";
    public ConfigEntry<string> WebsocketUrl { get; } = file.Bind(Section, nameof(WebsocketUrl), "ws://localhost:8000");
    public ConfigEntry<string> TwitchToken { get; } = file.Bind(Section, nameof(TwitchToken), "");
    public ConfigEntry<string> TwitchUsername { get; } = file.Bind(Section, nameof(TwitchUsername), "alexejherodev");
}
