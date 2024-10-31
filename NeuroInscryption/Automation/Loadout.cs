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
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Automation;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
internal sealed class Loadout
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class ItemsContext<TData>(IEnumerable<TData> items, object? additionalContext = null) : ContextBase(additionalContext)
        where TData : class, IIdentifiable
    {
        // ReSharper disable PossibleMultipleEnumeration
        [JsonIgnore]
        internal readonly IReadOnlyList<TData> Items = items.ToList();

        public readonly IReadOnlyList<string> Options = items.Select(i => i.Identifier).ToList();
        public readonly IDictionary<string, string>? Descriptions = items.OfType<IDescribable>().ToDescriptionDictionary().NullIfEmpty();
        // ReSharper restore PossibleMultipleEnumeration
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class ContextBase(object? additionalContext = null) : ILoadoutContext
    {
        [JsonExtensionData]
        public readonly JObject? AdditionalContext = additionalContext != null ? JObject.FromObject(additionalContext) : null;
        protected virtual IEnumerable<SigilData> RelevantSigils => [];
        protected virtual IEnumerable<TraitData> RelevantTraits => [];

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => RelevantSigils;
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => RelevantTraits;
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

    public async UniTask<T> ChooseItem<T>(IReadOnlyCollection<T> items, Description description, CancellationToken ct = default,
        object? additionalContext = null)
        where T : class, IIdentifiable
    {
        AutomationManager.Assert(items.Count > 0, "Nothing to pick from");
        if (items.Count == 1) return items.First();

        ChooseAction<T> chooseAction = new(items);

        ILoadoutContext context = additionalContext as ILoadoutContext ?? new ItemsContext<T>(items, additionalContext);

        return await Choose(chooseAction, description, ct, context);
    }

    /// <summary>
    /// Send a <see cref="Decision"/> that contains the given <paramref name="action"/> and <paramref name="additionalContext"/> and return the result from the response.
    /// </summary>
    /// <param name="description">Description to send. This should describe what kind of choice is being made.</param>
    /// <param name="additionalContext">
    /// Any additional context to include in the state.
    /// This is most commonly an anonymous object, but you can pass a custom <see cref="ILoadoutContext"/> instead.
    /// </param>
    /// <returns>A result of type <typeparamref name="T"/> that validates the given <paramref name="action"/>'s schema.</returns>
    public async UniTask<T> Choose<T>(IAction<T> action, Description description, CancellationToken ct = default,
        object? additionalContext = null)
    {
        LoadoutState state = LoadoutState.Calculate();
        state.SetContext(additionalContext as ILoadoutContext ?? new ContextBase(additionalContext));

        Decision decision = new(description, state);
        decision.AddAction(action);

        Action chosenAction = await decision.SendAsync(ct);

        if (!chosenAction.Match(action, out T? result)) chosenAction.ThrowInvalid();

        Plugin.Logger.LogDebug($"Picked {(result is IIdentifiable id ? id.Identifier : result.ToString())}");
        return result;
    }
}
