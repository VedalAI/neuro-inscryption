using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroInscryption.Reaction.API;

public sealed class VideoTracker(Dictionary<double, string> events)
{
    private double _lastTimeSent = -1;

    public void Send(double time)
    {
        StringBuilder prompt = new();

        foreach (KeyValuePair<double, string> videoEvent in events.OrderBy(kvp => kvp.Key))
        {
            if (_lastTimeSent < videoEvent.Key && videoEvent.Key <= time) prompt.Append(videoEvent.Value);
        }

        _lastTimeSent = time;

        if (prompt.Length <= 0) return;

        new Event(prompt.ToString(), Priority.Context).Send();
    }
}