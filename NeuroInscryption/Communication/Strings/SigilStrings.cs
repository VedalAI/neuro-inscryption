using System;
using System.Collections.Generic;
using DiskCardGame;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication.Strings;

public static class SigilStrings
{
    public const string MultipleMoveSigils = "This card has multiple movement sigils, so you can't tell which direction it will go in!";
    public const string MovingLeft = "This card's movement sigil indicates that it will move left (towards lane 1).";
    public const string MovingRight = "This card's movement sigil indicates that it will move right (towards lane 4).";
    
    private static readonly Dictionary<Ability, string> _nameOverrides = new()
    {
        [Ability.SubmergeSquid] = "Kraken Waterborne",
    };

    private static readonly Dictionary<Ability, string> _descriptionOverrides = new()
    {
        [Ability.DrawRabbits] = "When played, a Rabbit [0 power, 1 health, inherits this card's sigils] is created in your hand.",
        [Ability.BeesOnHit] = "When struck, a Bee [1 power, 1 health, Airborne] is created in your hand.",
        [Ability.Strafe] = "At the end of the owner's turn, attempts to move one space in the direction of the sigil.",
        [Ability.Deathtouch] = "When this card deals damage to another card, the other card dies.",
        [Ability.Evolve] = "Grows into a more powerful form after 1 full turn on the board.",
        [Ability.CreateDams] = "When played, plays Dams [0 power, 2 health] on adjacent spaces.",
        [Ability.Tutor] = "When played, you may pick and draw any card from your deck.",
        [Ability.WhackAMole] = "Intercepts opponent strikes on empty spaces by moving to those spaces to receive the strike.",
        [Ability.DrawCopy] = "When played, a copy of this card is created in your hand. The copy doesn't have this sigil.",
        [Ability.TailOnHit] = "When struck, this card attempts to move to an adjacent space (prioritizing the right side) and leave behind a Tail [base 0 power, 2 health; inherits this card's bonus stats and tribe].",
        [Ability.CorpseEater] = "When in hand, if an allied card dies on the board, this card will automatically play itself in its place at no cost.",
        [Ability.QuadrupleBones] = "On death, grants 4 bones instead of 1.",
        [Ability.Submerge] = "Submerges itself during its opponent's turn. While submerged, opposing creatures attack its owner directly.",
        [Ability.DrawCopyOnDeath] = "On death, a copy of this card is created in your hand.",
        [Ability.Sharp] = "When damaged, the attacker is dealt 1 damage.",
        [Ability.StrafePush] = "At the end of the owner's turn, moves one space in the direction of the sigil, pushing any creatures in the way.",
        [Ability.DrawAnt] = "When played, a Worker Ant [Ant Power, 1 health] is created in your hand.",
        [Ability.GuardDog] = "When an opponent card is played, this card moves to oppose it.",
        [Ability.Flying] = "Attacks strike the opponent directly, flying over any opposing cards.",
        [Ability.Sacrificial] = "Does not die when sacrificed.",
        [Ability.PreventAttack] = "Cannot be targeted by attacks.",
        [Ability.TripleBlood] = "Counts as 3 blood when sacrificed instead of 1.",
        [Ability.Reach] = "Blocks Airborne creatures, redirecting their strikes to itself instead of this card's owner.",
        [Ability.SplitStrike] = "Each attack strikes left of the opposing space, then to the right of it, but not directly forwards.",
        [Ability.TriStrike] = "Each attack strikes left of the opposing space, then the opposing space itself, then to the right of it.",
        [Ability.IceCube] = "On death, the creature inside replaces this card.",
        [Ability.RandomConsumable] = "When played, gives a random item. If your pack is full, the item is discarded.",
        [Ability.SteelTrap] = "On death, kills the opposing card and creates a Pelt in your hand.",
        [Ability.RandomAbility] = "When drawn, this sigil is replaced with a random one.",
        [Ability.SquirrelOrbit] = "Pulls small creatures like squirrels away into its orbit.",
        [Ability.AllStrike] = "Individually attacks all creatures on the opposing side. If no creatures oppose it, it attacks the opponent directly once.",
        [Ability.BuffNeighbours] = "Increases power of adjacent allies by 1.",
        [Ability.CreateBells] = "When played, plays Chimes [0 power, 1 health] on adjacent spaces.",
        [Ability.DebuffEnemy] = "Cards on the opposing space temporarily lose 1 power.",
        // kaycee's
        [Ability.Brittle] = "Dies after attacking.",
        [Ability.BuffEnemy] = "Cards on the opposing space gain 1 temporary power.",
        [Ability.SubmergeSquid] = "Submerges itself during its opponent's turn. While submerged, opposing creatures attack its owner directly. After emerging, this card will transform into a random Tentacle card.",
        [Ability.MadeOfStone] = "Immune to Touch of Death and Stinky.",
        [Ability.Morsel] = "When sacrificed in battle, adds its power and health to the card it is sacrificed for.",
        [Ability.BoneDigger] = "Grants 1 bone each turn.",
        [Ability.CreateEgg] = "When played, creates a Broken Egg [1 health, Terrain] (or, rarely, a Raven Egg [0 power, 2 health, Fledgling]) on the opposing space if it is empty.",
        [Ability.HydraEgg] = "Hatches into a Hydra [1 power, 5 health, Bifurcated Strike and Trifurcated Strike, belongs to all tribes] when drawn, but only if cards in your deck collectively: (1) represent the numbers 1 to 5 in their power values, (2) represent the numbers 1 to 5 in their health values, and (3) represent all 5 tribes (each Amalgam card counts for any one tribe, but only once)",
        [Ability.DoubleStrike] = "Each attack strikes the opposing space one additional time.",
        [Ability.DeathShield] = "Negates the first instance of damage taken.",
        [Ability.BloodGuzzler] = "On kill, gains 1 temporary power.",
        [Ability.OpponentBones] = "While in play, grants bones when opponents' creatures die.",
        [Ability.StrafeSwap] = "At the end of the owner's turn, moves one space in the direction of the sigil, switching places with any card in its way.",
    };

    private static readonly Description[] _descriptions;

    static SigilStrings()
    {
        Array abilities = Enum.GetValues(typeof(Ability));
        _descriptions = new Description[abilities.Length];

        for (int i = 0; i < abilities.Length; i++)
        {
            Ability ability = (Ability)abilities.GetValue(i);
            _nameOverrides.TryGetValue(ability, out string? name);
            _descriptionOverrides.TryGetValue(ability, out string? desc);

            if (name is null || desc is null)
            {
                AbilityInfo info = AbilitiesUtil.GetInfo(ability);
                if (info)
                {
                    name ??= info.rulebookName;
                    desc ??= info.LocalizedRulebookDescription.ParseDescription();
                }
                else
                {
                    if (ability is not (Ability.None or Ability.NUM_ABILITIES))
                        Plugin.Logger.LogWarning($"No AbilityInfo for {ability}, better hope nothing uses its description!");
                    name = ability.ToString();
                    desc = GenericStrings.MissingDescription;
                }
            }
            _descriptions[i] = new Description(name, desc);
        }
    }

    public static Description Get(Ability sigil) => _descriptions[(int) sigil];
}
