using System;
using DiskCardGame;

namespace NeuroInscryption.Configuration;

[Flags]
public enum ChallengeFlags
{
    // LetNeuroChoose = -1, // todo
    None = 0,
    NoHook = 1 << 1,
    SmallerBackpack = 1 << 2,
    NoClover = 1 << 3,
    PriceyPelts = 1 << 4,
    BossTotems = 1 << 5,
    TippedScales = 1 << 6,
    AllTotemBattles = 1 << 7,
    NoBossRares = 1 << 8,
    MoreDifficult1 = 1 << 9,
    MoreDifficult2 = 1 << 10,
    SingleCandle = 1 << 11,
    AnnoyingStarters = 1 << 12,
    SquirrelFish = 1 << 13,
    GrizzlyBosses = 1 << 14,
    FinalBoss = 1 << 15,
}

public static class ChallengeFlagsExtensions
{
    public static bool Includes(this ChallengeFlags flags, AscensionChallenge challenge, bool isSecondAppearance)
    {
        return challenge switch
        {
            AscensionChallenge.AllTotems => flags.HasFlag(ChallengeFlags.AllTotemBattles),
            AscensionChallenge.BossTotems => flags.HasFlag(ChallengeFlags.BossTotems),
            AscensionChallenge.BaseDifficulty => flags.HasFlag(!isSecondAppearance ? ChallengeFlags.MoreDifficult1 : ChallengeFlags.MoreDifficult2),
            AscensionChallenge.LessConsumables => flags.HasFlag(ChallengeFlags.SmallerBackpack),
            AscensionChallenge.NoClover => flags.HasFlag(ChallengeFlags.NoClover),
            AscensionChallenge.LessLives => flags.HasFlag(ChallengeFlags.SingleCandle),
            AscensionChallenge.ExpensivePelts => flags.HasFlag(ChallengeFlags.PriceyPelts),
            AscensionChallenge.NoBossRares => flags.HasFlag(ChallengeFlags.NoBossRares),
            AscensionChallenge.WeakStarterDeck => flags.HasFlag(ChallengeFlags.AnnoyingStarters),
            AscensionChallenge.StartingDamage => flags.HasFlag(ChallengeFlags.TippedScales),
            AscensionChallenge.SubmergeSquirrels => flags.HasFlag(ChallengeFlags.SquirrelFish),
            AscensionChallenge.GrizzlyMode => flags.HasFlag(ChallengeFlags.GrizzlyBosses),
            AscensionChallenge.NoHook => flags.HasFlag(ChallengeFlags.NoHook),
            AscensionChallenge.FinalBoss => flags.HasFlag(ChallengeFlags.FinalBoss),
            _ => false
        };
    }
}
