using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
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

        await Wait.SceneChanged(ct);

        List<ItemData> items = instance.slots.Select(ItemData.Create).ToList();
        ItemData item = await Neuro.Misc.ChooseWithLoadout(items, DecisionStrings.ChooseItem, ct);
        AutomationManager.Assert(item is {}, "did not pick item");
        AutomationManager.Assert(item.Slot != null, "picked item has no slot");

        await NeuroMouse.MoveAndClick(item.Slot, ct);
    }
}
