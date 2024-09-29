using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Configuration;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class ChallengesScreenFlow : HarmonyAutomationFlow<AscensionChallengeScreen>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(AscensionChallengeScreen), nameof(AscensionChallengeScreen.OnEnable))
        }
    ];

    protected override async UniTask ExecuteAsync(AscensionChallengeScreen target, CancellationToken ct)
    {
        await UniTask.Delay(1000, cancellationToken: ct);

        ChallengeFlags enabledChallenges = Plugin.Config.Challenges.Value;

        bool seenDifficulty = false;
        foreach (AscensionIconInteractable icon in target.icons.OrderBy(i => !i.transform.parent.name.Contains("Top")))
        {
            if (enabledChallenges.Includes(icon.Info.challengeType, seenDifficulty) != icon.activatedRenderer.enabled)
            {
                await NeuroMouse.MoveToAsync(icon, 200, ct);
                await NeuroMouse.ClickAsync(ct);
            }

            if (icon.Info.challengeType == AscensionChallenge.BaseDifficulty) seenDifficulty = true;
        }

        await NeuroMouse.MoveToAsync(target.continueButton, 500, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
