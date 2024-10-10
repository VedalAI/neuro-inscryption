namespace NeuroInscryption.Reaction.API;

public enum Priority
{
    Disable, // Will not be sent to Neuro
    Silent,  // Neuro will not react to the prompt, but it will still be added to her context window
    Context, // Neuro may or may not react to the prompt, her choice
    Force,   // Neuro will react to the prompt
}