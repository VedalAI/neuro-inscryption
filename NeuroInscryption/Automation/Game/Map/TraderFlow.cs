using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class TraderFlow : HarmonyAutomationFlow<IEnumerator>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(List<SelectableCard> cards, int amount) : ILoadoutContext
    {
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Select(SelectableCardData.Create).ToList();
        public readonly int NumberOfCardsToBuy = amount;

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => CardOptions.SelectMany(c => c.Card!.RelevantSigils);
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => CardOptions.SelectMany(c => c.Card!.Traits);
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(TradePeltsSequencer), nameof(TradePeltsSequencer.TradePelts))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(18)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator target, CancellationToken ct)
    {
        TradePeltsSequencer instance = target.GetThis<TradePeltsSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        LoadoutState state = LoadoutState.Calculate();
        Context context = new(instance.tradeCards, instance.peltCards.Count);
        state.SetContext(context);

        ChooseMultipleAction<SelectableCardData> chooseAction = new(context.CardOptions, context.NumberOfCardsToBuy);

        Decision decision = new(DecisionStrings.ChooseTraderCards.Format(context.NumberOfCardsToBuy), state);
        decision.AddAction(chooseAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseAction, out IEnumerable<SelectableCardData>? chosenCards)) action.ThrowInvalid();

        AllowManualInterruption = false;
        foreach (SelectableCardData card in chosenCards)
        {
            await NeuroMouse.MoveToAsync(card.TargetCard, 500, ct);
            await NeuroMouse.ClickAsync(250, ct);
        }
    }
}
