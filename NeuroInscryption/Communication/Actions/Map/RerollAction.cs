using DiskCardGame;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class RerollAction(MainInputInteractable clover) : EmptyAction(ActionStrings.Reroll)
{
    public override bool IsApplicable => clover && clover.Enabled && clover.gameObject.activeSelf;
}
