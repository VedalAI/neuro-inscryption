using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Automation.Game.Map;
using NeuroInscryption.Configuration;
using NeuroInscryption.Utilities;
using NeuroSdk;
using UnityEngine;
using UnityEngine.LowLevel;

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
        PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
        Cysharp.Threading.Tasks.PlayerLoopHelper.Initialize(ref loop);
        
        Logger = base.Logger;
        Config = new ModConfig(base.Config);
        PluginObject = gameObject;

        Singlyboi.AddAll(gameObject);

        Harmony.PatchAll();
    }

    private void Start()
    {
        NeuroSdkSetup.Initialize("Inscryption");
    }
}
