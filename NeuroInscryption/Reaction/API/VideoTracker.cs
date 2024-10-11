using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroInscryption.Reaction.API;

public sealed class VideoTracker(List<VideoTracker.VideoEvent> events)
{
    public record struct VideoEvent(double Timestamp, string Description)
    {
        public static implicit operator VideoEvent((double, string) tuple) => new(tuple.Item1, tuple.Item2);
    }
    private double _lastTimeSent = -1;

    public void Send(double time)
    {
        StringBuilder prompt = new();

        List<VideoEvent> newEvents = events
            .SkipWhile(kf => kf.Timestamp <= _lastTimeSent)
            .TakeWhile(kf => kf.Timestamp <= time)
            .ToList();

        bool continuing = _lastTimeSent != -1;
        if (continuing && newEvents.Count > 0)
            prompt.Append("The video continues. ");

        foreach (VideoEvent videoEvent in newEvents)
        {
            prompt.Append(videoEvent.Description);
        }

        _lastTimeSent = time;

        if (prompt.Length <= 0) return;

        string message = prompt.ToString();
        if (continuing)
        {
            // if continuing, it's not obvious who "he" is
            int luckyCarder = message.IndexOf("The Lucky Carder");
            int firstHe = message.IndexOf("He", System.StringComparison.OrdinalIgnoreCase);
            if (firstHe >= 0 && (luckyCarder < 0 || firstHe < luckyCarder))
                message = message.Remove(firstHe, 2).Insert(firstHe, "The Lucky Carder");
        }

        new Event(prompt.ToString(), Priority.Context).Send();
    }
}