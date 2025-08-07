using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class AcceptSurrenderAction(bool? isOpponentSurrendering) : BasicAction(ActionStrings.AcceptSurrender)
{
    public override bool IsApplicable => isOpponentSurrendering == true;
}
