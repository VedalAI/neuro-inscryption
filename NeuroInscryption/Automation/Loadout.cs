using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Data.State;

namespace NeuroInscryption.Automation;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
internal sealed class Loadout
{
    public record struct InteractableOption(EmptyAction Action, InteractableBase Interactable)
    {
        public static implicit operator InteractableOption((string ActionName, InteractableBase Interactable) tuple)
        {
            return new InteractableOption(new EmptyAction(tuple.ActionName), tuple.Interactable);
        }

        public static implicit operator InteractableOption((EmptyAction Action, InteractableBase Interactable) tuple)
        {
            return new InteractableOption(tuple.Action, tuple.Interactable);
        }
    }
    public async UniTask<InteractableBase> ChooseInteractable(InteractableOption[] options, string decisionQuery, IGameState state,
        CancellationToken ct = default)
    {
        AutomationManager.Assert(options.Length > 0, "Nothing to pick from");
        if (options.Length == 1) return options[0].Interactable;

        Decision decision = new(decisionQuery, state);
        foreach (InteractableOption option in options)
        {
            decision.AddAction(option.Action);
        }

        ActionBase pickedAction = await decision.Send(ct);

        InteractableOption chosenOption = Array.Find(options, opt => opt.Action == pickedAction);
        AutomationManager.Assert(chosenOption != default, "Sanity: picked non-existent option");

        Plugin.Logger.LogDebug($"Picked {chosenOption.Interactable} ({chosenOption.Action.Name})");

        return chosenOption.Interactable;
    }

    public UniTask<T> Choose<T>(ActionBase<T> action, string decisionQuery, CancellationToken ct = default, object? additionalContext = null)
    {
        return Neuro.Common.Choose(action, decisionQuery, LoadoutState.Calculate(), ct, additionalContext);
    }

    public UniTask<TData> ChooseItem<TData>(IReadOnlyCollection<TData> items, string decisionQuery, CancellationToken ct, object? additionalContext = null)
        where TData : class, IIdentifiable
    {
        return Neuro.Common.ChooseItem(items, decisionQuery, LoadoutState.Calculate(), ct, additionalContext);
    }
}
