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
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class TraderFlow : HarmonyAutomationFlow<object>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(List<SelectableCard> cards, int amount, IGameState state) : ILoadoutContext
    {
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Select((c, i) => SelectableCardData.Create(c, i, state)).ToList();
        public readonly int NumberOfCardsToBuy = amount;

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => CardOptions.SelectMany(c => c.Card!.RelevantSigils);
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => CardOptions.SelectMany(c => c.Card!.Traits);
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(TradePeltsSequencer), nameof(TradePeltsSequencer.TradePelts))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 18
        }
    ];

    protected override async UniTask ExecuteAsync(object target, CancellationToken ct)
    {
        TradePeltsSequencer instance = target.GetEnumeratorThis<TradePeltsSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        LoadoutState state = LoadoutState.Calculate();
        Context context = new(instance.tradeCards, instance.peltCards.Count, state);
        state.SetContext(context);

        ChooseTraderCardsAction chooseAction = new(context.CardOptions, context.NumberOfCardsToBuy);

        Decision decision = new(DecisionStrings.ChooseTraderCards.FormatPlural(context.NumberOfCardsToBuy), state);
        decision.AddAction(chooseAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseAction, out IEnumerable<SelectableCardData>? chosenCards)) action.ThrowInvalid();

        foreach (SelectableCardData card in chosenCards)
        {
            await NeuroMouse.MoveToAsync(card.TargetCard, 500, ct);
            AllowManualInterruption = false;
            await NeuroMouse.ClickAsync(250, ct);
        }
    }
}
