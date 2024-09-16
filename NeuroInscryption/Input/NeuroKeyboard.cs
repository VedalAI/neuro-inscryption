using System.Threading;
using Cysharp.Threading.Tasks;

namespace NeuroInscryption.Input;

public static class NeuroKeyboard
{
    public enum Button
    {
        //Select = 2,
        LookUp = 3,
        LookDown = 4,
        LookLeft = 5,
        LookRight = 6,
        //AltSelect = 7,
        Cancel = 8,
        EndTurn = 9,
        DirUp = 10,
        DirDown = 11,
        DirLeft = 12,
        DirRight = 13,
        Menu = 14,
        AltMenu = 16,
    }

    public static void Press(Button button)
    {
        InputHandler.EnqueueInput((global::Button) button, true);
        InputHandler.EnqueueInput((global::Button) button, false);
    }

    public static async UniTask Press(Button button, int msDelay, CancellationToken ct = default)
    {
        Press(button);
        await UniTask.Delay(msDelay, cancellationToken: ct);
    }
}
