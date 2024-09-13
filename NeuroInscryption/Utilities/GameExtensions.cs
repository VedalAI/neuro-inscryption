using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DiskCardGame;

namespace NeuroInscryption.Utilities;

public static class GameExtensions
{
    public static bool IsIdentical(this CardInfo? card1, CardInfo? card2)
    {
        if (card1 == null || card2 == null) return card1 == null && card2 == null;

        if (card1.DisplayedNameEnglish != card2.DisplayedNameEnglish) return false;
        if (card1.Attack != card2.Attack) return false;
        if (card1.Health != card2.Health) return false;
        if (card1.BloodCost != card2.BloodCost) return false;
        if (card1.BonesCost != card2.BonesCost) return false;
        if (card1.Abilities.Count != card2.Abilities.Count) return false;

        List<Ability> abilities1 = card1.Abilities.OrderBy(ability => ability).ToList();
        List<Ability> abilities2 = card2.Abilities.OrderBy(ability => ability).ToList();
        for (int i = 0; i < abilities1.Count; i++)
        {
            if (abilities1[i] != abilities2[i]) return false;
        }

        return true;
    }

    public static bool TryGetSpecialAbility<T>(this PlayableCard card, SpecialTriggeredAbility ability, [NotNullWhen(true)] out T? behaviour) where T : SpecialCardBehaviour
    {
        behaviour = (T?) card.TriggerHandler.specialAbilities.Find(t => t?.Item1 == ability)?.Item2;
        return behaviour != null;
    }
}
