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

        if (_lastTimeSent != -1 && newEvents.Count > 0)
            prompt.Append("The video continues. ");

        // TODO: replace first "He" with "The Lucky Carder"
        foreach (VideoEvent videoEvent in newEvents)
        {
            prompt.Append(videoEvent.Description);
        }

        _lastTimeSent = time;

        if (prompt.Length <= 0) return;

        new Event(prompt.ToString(), Priority.Context).Send();
    }
}