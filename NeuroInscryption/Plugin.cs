using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using NeuroInscryption.Configuration;
using NeuroInscryption.Utilities;
using NeuroSdk;
using UnityEngine;

namespace NeuroInscryption;

[BepInAutoPlugin]
public sealed partial class Plugin : BaseUnityPlugin
{
    public static Harmony Harmony { get; } = new(Id);
    public new static ManualLogSource Logger { get; private set; } = null!;
    public new static ModConfig Config { get; private set; } = null!;
    public static GameObject PluginObject { get; private set; } = null!;

    private void Awake()
    {
        Logger = base.Logger;
        Config = new ModConfig(base.Config);
        PluginObject = gameObject;

        Singlyboi.AddAll(gameObject);

        Harmony.PatchAll();

        // TODO: twitch integration to change the challenges and starter deck
    }

    private void Start()
    {
        NeuroSdkSetup.Initialize("Inscryption");
    }
}
