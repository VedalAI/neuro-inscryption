using System.Collections.Generic;
using System.Linq;
using DiskCardGame;

namespace NeuroInscryption.Utilities;

internal static class GameExtensions
{
    public static bool IsIdentical(this CardInfo card1, CardInfo card2)
    {
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

    public static (string name, string description) GetRulebookStrings(this Ability ability)
    {
        if (ability == Ability.SubmergeSquid) return ("Kraken Waterborne", "A card bearing this sigil submerges itself during its opponent's turn. While submerged, opposing creatures attack its owner directly. After emerging, this card will turn into a random Tentacle card.");
        AbilityInfo x = AbilitiesUtil.GetInfo(ability);
        return (x.rulebookName, x.LocalizedRulebookDescription.Replace("[creature]", Localization.Translate("a card bearing this sigil")));
    }
}
