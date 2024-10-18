using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using NeuroInscryption.Configuration;
using NeuroInscryption.Resources;
using NeuroInscryption.Utilities;
using UnityEngine.LowLevel;

namespace NeuroInscryption;

[BepInAutoPlugin]
public sealed partial class Plugin : BaseUnityPlugin
{
    public static Harmony Harmony { get; } = new(Id);
    public new static ManualLogSource Logger { get; private set; } = null!;
    public new static ModConfig Config { get; private set; } = null!;

    private void Awake()
    {
        Logger = base.Logger;
        Config = new ModConfig(base.Config);

        ResourceManager.InjectAssemblies();
        InitializeUniTask();
        Singlyboi.AddAll(gameObject);

        Harmony.PatchAll();

        // TODO: twitch integration to change the challenges and starter deck
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void InitializeUniTask()
    {
        PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
        PlayerLoopHelper.Initialize(ref loop);
    }
}
