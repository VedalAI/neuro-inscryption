using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
internal class Misc
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class LoadoutContext<TData>(IEnumerable<TData> items) : ILoadoutContext
        where TData : class, IIdentifiable
    {
        public readonly IReadOnlyList<TData> Options = items.ToList();
        public object? AdditionalContext { get; init; }

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

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
    public async UniTask<InteractableBase> ChooseInteractable(Description description, IGameState state, InteractableOption[] options, CancellationToken ct = default)
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

    public async UniTask<T> ChooseWithLoadout<T>(IReadOnlyCollection<T> items, Description description, CancellationToken ct = default,
        object? additionalContext = null)
        where T : class, IIdentifiable
    {
        AutomationManager.Assert(items.Count > 0, "Nothing to pick from");
        if (items.Count == 1) return items.First();

        LoadoutState state = LoadoutState.Calculate();
        LoadoutContext<T> context = new(items) { AdditionalContext = additionalContext };
        state.SetContext(context);

        ChooseAction<T> chooseAction = new(context.Options);
        Decision decision = new(description, state);
        decision.AddAction(chooseAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseAction, out T? selected)) action.ThrowInvalid();

        Plugin.Logger.LogDebug($"Picked {selected.Identifier}");

        return selected;
    }
}
