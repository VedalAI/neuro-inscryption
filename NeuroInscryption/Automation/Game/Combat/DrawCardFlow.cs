using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Combat;

public sealed class DrawCardFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardDrawPiles3D), nameof(CardDrawPiles3D.ChooseDraw))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(2)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator _, CancellationToken ct)
    {
        await Wait.CommonFlowDelay(ct);

        bool mainDeckEmpty = CardDrawPiles3D.Instance.Deck.Cards.Count == 0;
        bool sideDeckEmpty = CardDrawPiles3D.Instance.SideDeck.Cards.Count == 0;

        AsyncActionWindow window = new(DecisionStrings.DrawCard, CombatState.Calculate());
        window.AddAction(new DrawCardAction(mainDeckEmpty, sideDeckEmpty));
        DrawCardAction drawCardAction = new(mainDeckEmpty, sideDeckEmpty);
        return await Choose(drawCardAction, , ct, includeDeck: true);
        
        bool drawFromSideDeck = await Neuro.Combat.ChooseDrawFromSideDeck(ct);

        
    }
}
