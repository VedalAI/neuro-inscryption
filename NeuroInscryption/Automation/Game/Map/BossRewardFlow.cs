using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public class BossRewardFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new FlowHook()
        {
            Method = AccessTools.EnumeratorMoveNext(typeof(RareCardChoicesSequencer).GetMethod(nameof(RareCardChoicesSequencer.ChooseRareCard))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(8)
        }
    ];
    
    protected override async UniTask ExecuteAsync(IEnumerator target, CancellationToken ct)
    {
        EnumeratorWrapper<RareCardChoicesSequencer> wrapper = new(target);
        SelectableCard? card = await Neuro.Cards.ChooseAddToDeck(wrapper.This, ct);

        await NeuroMouse.MoveToAsync(card, 300, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}