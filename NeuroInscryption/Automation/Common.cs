using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Automation;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
public sealed class Common
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class ContextBase(object? additionalContext = null) : ILoadoutContext, ICombatContext
    {
        [JsonExtensionData]
        public readonly JObject? AdditionalContext = additionalContext != null ? JObject.FromObject(additionalContext) : null;
        protected virtual IEnumerable<SigilData> RelevantSigils => [];
        protected virtual IEnumerable<TraitData> RelevantTraits => [];

        [JsonIgnore]
        public virtual bool IncludeDeck => false;

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => RelevantSigils;
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => RelevantTraits;
    }

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

    /// <summary>
    /// Send a <see cref="Decision"/> that contains the given <paramref name="action"/> and <paramref name="additionalContext"/> and return the result from the response.
    /// </summary>
    /// <param name="decisionQuery">Query to send. This should describe what kind of choice is being made.</param>
    /// <param name="additionalContext">
    /// Any additional context to include in the state.
    /// This can be an anonymous object, or you can pass a custom <see cref="ILoadoutContext"/>/<see cref="ICombatContext"/> instead.
    /// </param>
    /// <param name="loadout">
    /// Whether to use the <see cref="LoadoutState"/> (if <see langword="true"/>)
    /// or the <see cref="CombatState"/> (if <see langword="false"/>).
    /// </param>
    /// <returns>A result of type <typeparamref name="T"/> that validates the given <paramref name="action"/>'s schema.</returns>
    public async UniTask<T> Choose<T>(ActionBase<T> action, string decisionQuery, IGameState state, CancellationToken ct = default,
        object? additionalContext = null)
    {
        switch (state)
        {
            case LoadoutState loadout:
                loadout.SetContext(additionalContext as ILoadoutContext ?? new ContextBase(additionalContext));
                break;
            case CombatState combat:
                combat.SetContext(additionalContext as ICombatContext ?? new ContextBase(additionalContext));
                break;
            default:
                throw new ArgumentException($"Invalid state - must be {nameof(LoadoutState)} or {nameof(CombatState)}", nameof(state));
        }

        Decision decision = new(decisionQuery, state);
        decision.AddAction(action);

        await decision.Send(ct);

        if (!action.Matches(decision, out T? result)) decision.ThrowInvalid();

        Plugin.Logger.LogDebug($"Picked {(result is IIdentifiable id ? id.Identifier : result.ToString())}");
        return result;
    }

    public async UniTask<T> Choose<T>(ActionBase<T> action, string decisionQuery, CancellationToken ct = default,
        object? additionalContext = null, bool useLoadoutState = true)
    {
        return await Choose(action, decisionQuery,
            useLoadoutState ? LoadoutState.Calculate() : CombatState.Calculate(),
            ct, additionalContext
        );
    }

    public async UniTask<TData> ChooseItem<TData>(IReadOnlyCollection<TData> items, string decisionQuery, IGameState state,
        CancellationToken ct = default, object? additionalContext = null)
        where TData : class, IIdentifiable
    {
        AutomationManager.Assert(items.Count > 0, "Nothing to pick from");
        if (items.Count == 1) return items.First();

        ChooseAction<TData> chooseAction = new(items);

        object context = additionalContext is ILoadoutContext or ICombatContext ? additionalContext
            : new ItemsContext<TData>(items, additionalContext);

        return await Choose(chooseAction, decisionQuery, state, ct, context);
    }
}
