using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using NeuroInscryption.Configuration;
using NeuroInscryption.Resources;
using NeuroInscryption.Utilities;

namespace NeuroInscryption;

[BepInAutoPlugin]
public sealed partial class Plugin : BaseUnityPlugin
{
    public static Harmony Harmony { get; } = new(Id);
    public new static ManualLogSource Logger { get; private set; }
    public new static ModConfig Config { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;
        Config = new ModConfig(base.Config);

        ResourceManager.InjectAssemblies();
        Singlyboi.AddAll(gameObject);

        Harmony.PatchAll();
    }
}
