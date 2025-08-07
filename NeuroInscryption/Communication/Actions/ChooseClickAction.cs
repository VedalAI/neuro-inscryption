using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;

namespace NeuroInscryption.Communication.Actions;

public class ChooseClickAction<T>(IEnumerable<T> items, Func<T, InteractableBase> getInteractable, int waitMs, string name = ActionStrings.MakeChoice) 
    : ChooseAction<T>(items, (item, ct) => Handler(item, ct, waitMs, getInteractable), name)
    where T : class, IIdentifiable
{
    private static UniTask Handler(T target, CancellationToken ct, int waitMs, Func<T, InteractableBase> getInteractable)
    {
        return NeuroMouse.MoveAndClick(getInteractable(target), ct, msDelayBeforeClick: waitMs);
    }
}