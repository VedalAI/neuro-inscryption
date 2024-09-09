using NeuroInscryption.Communication.Json;

namespace NeuroInscryption.Communication;

internal class FightDecision : Decision
{
    protected override object State { get; }
    protected override string Query => "Choose the next action to take during this card battle.";
    protected override JsonSchema ResponseSchema { get; }
}
