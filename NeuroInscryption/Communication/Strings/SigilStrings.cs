using System.Collections.Generic;
using DiskCardGame;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication.Strings;

public static class SigilStrings
{
    private const string SubmergeDescription = "Submerges itself during its opponent's turn. While submerged, opposing creatures attack its owner directly.";
    private static readonly Dictionary<Ability, string> _nameOverrides;
    private static readonly Dictionary<Ability, string> _descriptionOverrides;
    public static Description[] Descriptions { get; }

    static SigilStrings()
    {
        _nameOverrides = new()
        {
            [Ability.SubmergeSquid] = "Kraken Waterborne",
        };
        _descriptionOverrides = new()
        {
            [Ability.DrawRabbits] = "When played, a Rabbit [0 power, 1 health, inherits this card's sigils] is created in your hand.",
            [Ability.BeesOnHit] = "When struck, a Bee [1 power, 1 health, Airborne] is created in your hand.",
            [Ability.Strafe] = "At the end of the owner's turn, attempts to move one space in the direction of the sigil.",
            [Ability.Deathtouch] = "When this card deals damage, the target dies.",
            [Ability.Evolve] = "Grows into a more powerful form after 1 full turn on the board. After growing, Bone Digger effect is doubled.", // what a weird interaction
            [Ability.CreateDams] = "When played, plays Dams [0 power, 2 health] on adjacent spaces.",
            [Ability.Tutor] = "When played, you may pick and draw any card from your deck.",
            [Ability.WhackAMole] = "Intercepts opponent strikes on empty spaces by moving to those spaces to receive the strike.",
            [Ability.DrawCopy] = "When played, a copy of this card is created in your hand.",
            [Ability.TailOnHit] = "When struck, this card attempts to move to an adjacent space and leave behind a Tail [base 0 power, 2 health; inherits this card's bonus stats and tribe].",
            [Ability.CorpseEater] = "When in hand, if an allied card dies on the board, this card will automatically play itself in its place at no cost.",
            [Ability.QuadrupleBones] = "On death, grants 4 bones instead of 1.",
            [Ability.Submerge] = SubmergeDescription,
            [Ability.DrawCopyOnDeath] = "On death, a copy of this card is created in your hand.",
            [Ability.Sharp] = "When damaged, the attacker is dealt 1 damage.",
            [Ability.StrafePush] = "At the end of the owner's turn, moves one space in the direction of the sigil, pushing any creatures in the way.",
            [Ability.DrawAnt] = "When played, an Ant Worker [Ant Power, 1 health] is created in your hand.",
            [Ability.GuardDog] = "When an opponent card is played, this card moves to oppose it.",
            [Ability.Flying] = "Attacks strike the opponent directly, flying over any opposing cards.",
            [Ability.Sacrificial] = "Does not die when sacrificed.",
            [Ability.PreventAttack] = "Cannot be attacked.",
            [Ability.TripleBlood] = "Counts as 3 blood when sacrificed instead of 1.",
            [Ability.Reach] = "Blocks Airborne creatures, redirecting their strikes to itself instead of this card's owner.",
            [Ability.SplitStrike] = "Each attack strikes to the left and right of the opposing space, but not directly forwards.",
            [Ability.TriStrike] = "Each attack strikes left of the opposing space, then the space directly, then to the right of it.",
            [Ability.IceCube] = "On death, the creature inside replaces this card.",
            [Ability.RandomConsumable] = "When played, gives a random item. If your pack is full, the item is discarded.",
            [Ability.SteelTrap] = "On death, kills the opposing card and creates a Pelt in your hand.",
            [Ability.RandomAbility] = "When drawn, this sigil is replaced with a random one.",
            [Ability.SquirrelOrbit] = "Pulls small creatures like squirrels away into its orbit.",
            [Ability.AllStrike] = "Attacks strike all creatures on the opposing side. If no creatures oppose it, attacks will strike the opponent directly.",
            [Ability.BuffNeighbours] = "Increases power of adjacent allies by 1.",
            [Ability.CreateBells] = "When played, plays Chimes [0 power, 1 health] on adjacent spaces.",
            [Ability.DebuffEnemy] = "While in play, cards on the opposing space lose 1 power.",
            // kaycee's
            [Ability.Brittle] = "Dies after attacking.",
            [Ability.BuffEnemy] = "While in play, cards on the opposing space gain 1 power.",
            [Ability.SubmergeSquid] = $"{SubmergeDescription} After emerging, this card will transform into a random Tentacle card.",
            [Ability.MadeOfStone] = "Immune to Touch of Death and Stinky.",
            [Ability.Morsel] = "When sacrificed in battle, adds its power and health to the card it is sacrificed for.",
            [Ability.BoneDigger] = "When on your side of the board, grants 1 bone each turn.",
            [Ability.CreateEgg] = "When played, creates a Broken Egg [1 health, Terrain] (or, rarely, a Raven Egg [0 power, 2 health, Fledgling]) on the opposing space if it is empty.",
            [Ability.HydraEgg] = "Hatches into a Hydra [1 power, 5 health, Bifurcated Strike and Trifurcated Strike, belongs to all tribes] when drawn, but only if cards in your deck collectively: (1) represent the numbers 1 to 5 in their power values, (2) represent the numbers 1 to 5 in their health values, and (3) represent all 5 tribes (each Amalgam card counts for any one tribe, but only once)",
            [Ability.DoubleStrike] = "Attacks strike twice.",
            [Ability.DeathShield] = "Negates the first instance of damage taken.",
            [Ability.BloodGuzzler] = "On kill, gains 1 temporary power.",
            [Ability.OpponentBones] = "While in play, grants bones when opponents' creatures die.",
            [Ability.StrafeSwap] = "At the end of the owner's turn, moves one space in the direction of the sigil, switching places with any card in its way.",
        };
        System.Array abilities = typeof(Ability).GetEnumValues();
        Descriptions = new Description[abilities.Length];
        for (int i = 0; i < abilities.Length; i++)
        {
            Ability ability = (Ability)abilities.GetValue(i);
            _nameOverrides.TryGetValue(ability, out string? name);
            _descriptionOverrides.TryGetValue(ability, out string? desc);
            if (name is null || desc is null)
            {
                AbilityInfo info = AbilitiesUtil.GetInfo(ability);
                string rulebookName, rulebookDesc;
                if (info)
                {
                    rulebookName = info.rulebookName;
                    rulebookDesc = info.LocalizedRulebookDescription;
                    if (!string.IsNullOrEmpty(rulebookDesc))
                        rulebookDesc = rulebookDesc.ParseDescription();
                }
                else
                {
                    if (ability is not (Ability.None or Ability.NUM_ABILITIES))
                        Plugin.Logger.LogWarning($"No AbilityInfo for {ability}, better hope nothing uses its Description");
                    rulebookName = ability.ToString();
                    rulebookDesc = "Description missing.";
                }
                name ??= rulebookName;
                desc ??= rulebookDesc;
            }
            Descriptions[i] = new Description(name, desc);
        }
    }

    public static Description GetDescription(Ability sigil) => Descriptions[(int)sigil];
}
