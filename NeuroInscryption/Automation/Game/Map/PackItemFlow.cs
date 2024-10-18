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
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;
using ItemData = NeuroInscryption.Communication.Data.Serializable.ItemData;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class PackItemFlow : HarmonyAutomationFlow<IEnumerator>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(IEnumerable<SelectableItemSlot> items) : ILoadoutContext
    {
        public readonly IReadOnlyList<ItemData> ItemOptions = items.Select(ItemData.Create).ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(GainConsumablesSequencer), nameof(GainConsumablesSequencer.RegularGainConsumables))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(5)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        GainConsumablesSequencer instance = enumerator.GetThis<GainConsumablesSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        Context context = new(instance.slots);
        LoadoutState state = LoadoutState.Calculate();
        state.SetContext(context);

        ChooseAction<ItemData> chooseAction = new(context.ItemOptions, true);

        Decision decision = new(DecisionStrings.ChooseItem, state);
        decision.AddAction(chooseAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseAction, out ItemData? item)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(item.Slot!, 300, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
