using System.Collections;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace NeuroInscryption.Tweaks;

[HarmonyPatch]
internal sealed class DevCheats : Singlyboi<DevCheats>
{
    private void Start()
    {
#if !DEBUG
        enabled = false;
#endif
    }

    private void Update()
    {
        // Ctrl+Q ("quick win")
        // Ctrl+Shift+Q (or spam Ctrl+Q) to skip remaining phases (on bosses)
        if (UInput.GetKey(KeyCode.LeftControl) && UInput.GetKeyDown(KeyCode.Q))
        {
            Win(UInput.GetKey(KeyCode.LeftShift));
        }
        if (UInput.GetKey(KeyCode.LeftAlt) && UInput.GetKeyDown(KeyCode.Q))
        {
            ToggleAutoWin();
        }
        // Ctrl+Shift+4 ($)
        if (UInput.GetKey(KeyCode.LeftControl) && UInput.GetKey(KeyCode.LeftShift) && UInput.GetKeyDown(KeyCode.Alpha4))
        {
            RunState.Run.currency++;
        }

        if (UInput.GetKeyDown(KeyCode.RightBracket))
        {
            Faster();
        }

        if (UInput.GetKeyDown(KeyCode.LeftBracket))
        {
            Slower();
        }
    }

    private float _lastWon;
    /// <summary>
    /// Instantly tip the scales to kill the opponent.<br/>
    /// Calling this twice within one second (or setting <paramref name="winHarder"/>) will skip any remaining phases (for bosses).
    /// </summary>
    public void Win(bool winHarder = false)
    {
        if (!GameFlowManager.IsCardBattle)
        {
            Plugin.Logger.LogWarning("(Win cheat) Not in a battle");
            return;
        }

        bool isForced = winHarder;
        if (_lastWon + 1 > Time.time)
            winHarder = true;
        _lastWon = Time.time;

        if (winHarder) TurnManager.Instance.Opponent.NumLives = 1;
        
        string msg = "winning";
        if (winHarder)
        {
            msg += " harder";
            msg += isForced ? " (forced)" : " (spammed)";
        }
        Plugin.Logger.LogInfo(msg);

        StartCoroutine(LifeManager.Instance.ShowDamageSequence(LifeManager.Instance.DamageUntilPlayerWin, 1, false,
            alternateWeightPrefab: ResourceBank.Get<GameObject>("Prefabs/Environment/ScaleWeights/Weight_DataFile_GB")));
    }

    public static void Faster() => Time.timeScale += 1;
    public static void Slower() => Time.timeScale -= 1;
    
    public void ToggleAutoWin(bool? overrideValue = null)
    {
        _doAutoWin = overrideValue ?? !_doAutoWin;
        Plugin.Logger.LogInfo($"autowin {(_doAutoWin ? "on" : "off")}");
        if (_doAutoWin) Win(true);
    }

    private static bool _doAutoWin;

    [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.GameSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void AutoWin(IEnumerator __instance)
    {
        if (_doAutoWin && __instance.GetState() == 2) // setup phase
            Instance.Win(true);
    }
}
