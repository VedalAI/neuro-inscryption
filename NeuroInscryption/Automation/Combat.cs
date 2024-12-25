using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json;

namespace NeuroInscryption.Automation;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
public sealed class Combat
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class IncludeDeckOverride(bool includeDeck, ICombatContext originalContext)
        : Common.ContextBase(originalContext)
    {
        [JsonIgnore]
        public override bool IncludeDeck => includeDeck;
    }
    public async UniTask<bool> ChooseDrawFromSideDeck(CancellationToken ct = default)
    {
        bool mainDeckEmpty = CardDrawPiles3D.Instance.Deck.Cards.Count == 0;
        bool sideDeckEmpty = CardDrawPiles3D.Instance.SideDeck.Cards.Count == 0;

        if (mainDeckEmpty)
        {
            Plugin.Logger.LogMessage($"Main deck empty, force draw from side");
            return true;
        }
        if (sideDeckEmpty)
        {
            Plugin.Logger.LogMessage($"Side deck empty, force draw from main");
            return false;
        }
        DrawCardAction drawCardAction = new(mainDeckEmpty, sideDeckEmpty);
        return await Choose(drawCardAction, DecisionStrings.DrawCard, ct, includeDeck: true);
    }

    public UniTask<T> Choose<T>(ActionBase<T> action, Description description, CancellationToken ct = default, object? additionalContext = null, bool? includeDeck = null)
    {
        if (includeDeck.HasValue)
        {
            ICombatContext combatContext = additionalContext as ICombatContext ?? new Common.ContextBase(additionalContext);
            additionalContext = new IncludeDeckOverride(includeDeck.Value, combatContext);
        }
        return Neuro.Common.Choose(action, description, CombatState.Calculate(), ct, additionalContext);
    }

    public UniTask<TData> ChooseItem<TData>(IReadOnlyCollection<TData> items, Description description, CancellationToken ct = default,
        object? additionalContext = null, bool? includeDeck = null)
        where TData : class, IIdentifiable
    {
        if (includeDeck.HasValue)
        {
            ICombatContext combatContext = additionalContext as ICombatContext ?? new Common.ContextBase(additionalContext);
            additionalContext = new IncludeDeckOverride(includeDeck.Value, combatContext);
        }
        return Neuro.Common.ChooseItem(items, description, CombatState.Calculate(), ct, additionalContext);
    }
}
