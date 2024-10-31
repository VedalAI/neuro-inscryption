using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Outgoing;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Automation;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
internal sealed class Loadout
{
    public record struct InteractableOption(BasicAction Action, InteractableBase Interactable)
    {
        public static implicit operator InteractableOption((string ActionName, InteractableBase Interactable) tuple)
        {
            return new InteractableOption(new BasicAction(tuple.ActionName), tuple.Interactable);
        }

        public static implicit operator InteractableOption((BasicAction Action, InteractableBase Interactable) tuple)
        {
            return new InteractableOption(tuple.Action, tuple.Interactable);
        }
    }
    public async UniTask<InteractableBase> ChooseInteractable(InteractableOption[] options, Description description, IGameState state,
        CancellationToken ct = default)
    {
        AutomationManager.Assert(options.Length > 0, "Nothing to pick from");
        if (options.Length == 1) return options[0].Interactable;

        Decision decision = new(description, state);
        foreach (InteractableOption option in options)
        {
            decision.AddAction(option.Action);
        }

        Action chosenAction = await decision.SendAsync(ct);

        InteractableOption chosenOption = Array.Find(options, opt => chosenAction.Match(opt.Action));
        if (chosenOption == default) chosenAction.ThrowInvalid();

        Plugin.Logger.LogDebug($"Picked {chosenOption.Interactable} ({chosenOption.Action.ActionName})");

        return chosenOption.Interactable;
    }

    public UniTask<T> Choose<T>(IAction<T> action, Description description, CancellationToken ct = default, object? additionalContext = null)
    {
        return Neuro.Common.Choose(action, description, LoadoutState.Calculate(), ct, additionalContext);
    }

    public UniTask<TData> ChooseItem<TData>(IReadOnlyCollection<TData> items, Description description, CancellationToken ct, object? additionalContext = null)
        where TData : class, IIdentifiable
    {
        return Neuro.Common.ChooseItem(items, description, LoadoutState.Calculate(), ct, additionalContext);
    }
}
