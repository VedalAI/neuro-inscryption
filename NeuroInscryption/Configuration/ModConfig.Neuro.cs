using BepInEx.Configuration;

namespace NeuroInscryption.Configuration;

partial class ModConfig
{
    public ConfigEntry<float> MouseSpeedMultiplier { get; } = file.Bind("Configuration", "MouseSpeedMultiplier", 1f);

    public ConfigEntry<StarterDeckOption> StarterDeck { get; } = file.Bind("Configuration", "StarterDeck", StarterDeckOption.Vanilla);

    public ConfigEntry<ChallengeFlags> Challenges { get; } = file.Bind("Configuration", "Challenges", ChallengeFlags.None);
}