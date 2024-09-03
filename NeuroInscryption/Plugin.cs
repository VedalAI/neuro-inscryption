using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using NeuroInscryption.API.Utilities;
using NeuroInscryption.Configuration;

namespace NeuroInscryption;

[BepInAutoPlugin]
public sealed partial class Plugin : BaseUnityPlugin
{
    internal static Harmony Harmony { get; } = new(Id);
    internal new static ManualLogSource Logger { get; private set; }
    internal new static ModConfig Config { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;

        Config = new ModConfig(base.Config);
        Singlyboi.AddAll(gameObject);

        Harmony.PatchAll();
    }
}
