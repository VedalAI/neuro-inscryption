using System.Collections;
using DiskCardGame;
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
        if (UInput.GetKey(KeyCode.LeftControl) && UInput.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(Win(UInput.GetKey(KeyCode.LeftShift)));
        }
    }

    private float _lastWon;
    /// <summary>
    /// Instantly tip the scales to kill the opponent.<br/>
    /// Calling this twice within one second (or setting <paramref name="winHarder"/>) will skip any remaining phases (for bosses).
    /// </summary>
    public IEnumerator Win(bool winHarder = false)
    {
        if (TurnManager.Instance is not { Opponent: { } opponent }) yield break;

        if (_lastWon + 1 > Time.time)
            winHarder = true;
        _lastWon = Time.time;

        if (winHarder) opponent.NumLives = 1;

        // to-do: find funny prefab to throw on the scales
        yield return LifeManager.Instance.ShowDamageSequence(LifeManager.Instance.DamageUntilPlayerWin, 1, false);
    }
}
