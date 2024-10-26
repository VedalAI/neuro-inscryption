using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Outgoing;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Automation;

internal class Misc
{
    public record struct InteractableOption(BasicAction Action, InteractableBase Interactable)
    {
        public static implicit operator InteractableOption((string ActionName, InteractableBase Interactable) tuple)
        {
            return new InteractableOption(new BasicAction(tuple.ActionName), tuple.Interactable);
        }

        public static implicit operator InteractableOption((BasicAction Action, InteractableBase Interactable) tuple)
        {
            return new InteractableOption(tuple.Action, tuple.Interactable);
        }
    }
    public async UniTask<InteractableBase> ChooseInteractable(Description description, IGameState state, InteractableOption[] options, CancellationToken ct = default)
    {
        Decision decision = new(description, state);
        foreach (InteractableOption option in options)
        {
            decision.AddAction(option.Action);
        }

        Action chosenAction = await decision.SendAsync(ct);
        
        InteractableOption chosenOption = Array.Find(options, opt => chosenAction.Match(opt.Action));
        if (chosenOption == default) chosenAction.ThrowInvalid();

        return chosenOption.Interactable;
    }
}