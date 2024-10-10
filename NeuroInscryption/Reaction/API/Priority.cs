namespace NeuroInscryption.Reaction.API;

public enum Priority
{
    Force,   // Neuro will react to the prompt
    Context, // Neuro may or may not react to the prompt, her choice
    Silent   // Neuro will not react to the prompt, but it will still be added to her context window
}