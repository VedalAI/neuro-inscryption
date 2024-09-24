using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Configuration;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class StarterDeckScreenFlow : AutomationFlow<AscensionChooseStarterDeckScreen>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(AscensionChooseStarterDeckScreen), nameof(AscensionChooseStarterDeckScreen.OnEnable))
        }
    ];

    public override bool CanInterruptItself => true;

    protected override async UniTask ExecuteAsync(AscensionChooseStarterDeckScreen target, CancellationToken ct)
    {
        await UniTask.Delay(1000, cancellationToken: ct);

        StarterDeckOption starterDeck = Plugin.Config.StarterDeck.Value;

        string? targetDeck = starterDeck switch
        {
            StarterDeckOption.Vanilla => "Vanilla",
            StarterDeckOption.HighCost => "Moose Blood",
            StarterDeckOption.Ant => "Ants",
            StarterDeckOption.MantisGod => "One True God",
            StarterDeckOption.Waterborne => "Submersibles",
            StarterDeckOption.Bone => "Bones",
            StarterDeckOption.Free => "Free Reptiles",
            StarterDeckOption.Hydra => "Eggs",
            _ => null
        };

        if (string.IsNullOrEmpty(targetDeck))
        {
            await NeuroMouse.MoveToAsync(target.randomButton, 500, ct);
            await NeuroMouse.ClickAsync(ct);
            return;
        }

        AscensionStarterDeckIcon deck = target.deckIcons.First(d => d.Info.title == targetDeck);
        await NeuroMouse.MoveToAsync(deck, 500, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
