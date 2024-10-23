using System.Collections;
using DiskCardGame;
using GBC;
using NeuroInscryption.Utilities;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace NeuroInscryption.Tweaks;
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
        // Ctrl(+Shift)+Q ("quick win")
        if (UInput.GetKey(KeyCode.LeftControl) && UInput.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(Win(UInput.GetKey(KeyCode.LeftShift)));
        }
        // Ctrl+Shift+4 ($)
        if (UInput.GetKey(KeyCode.LeftControl) && UInput.GetKey(KeyCode.LeftShift) && UInput.GetKeyDown(KeyCode.Alpha4))
        {
            RunState.Run.currency++;
            SaveData.Data.currency++;
            Part3SaveData.Data.currency++;
        }
    }

    private float _lastWon;
    /// <summary>
    /// Instantly tip the scales to kill the opponent.<br/>
    /// Calling this twice within one second (or setting <paramref name="winHarder"/>) will skip any remaining phases (for bosses).
    /// </summary>
    public IEnumerator Win(bool winHarder = false)
    {
        if (!TurnManager.Instance.Opponent) yield break;

        if (_lastWon + 1 > Time.time)
            winHarder = true;
        _lastWon = Time.time;

        if (winHarder) TurnManager.Instance.Opponent.NumLives = 1;

        yield return LifeManager.Instance.ShowDamageSequence(LifeManager.Instance.DamageUntilPlayerWin, 1, false,
            alternateWeightPrefab: ResourceBank.Get<GameObject>("Prefabs/Environment/ScaleWeights/Weight_DataFile_GB"));
    }
}
