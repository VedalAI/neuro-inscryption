using System.Collections.Generic;
using NeuroInscryption.Reaction.API;

namespace NeuroInscryption.Reaction;

public static class Videos
{
    public static readonly VideoTracker Video1 = new(new Dictionary<double, string>
    {
        [0] = "A video dated 15th of September started. It shows a young man adjusting a camera in front of a green screen background and a table. On top of the table there are unopened packs of cards."
    });

    public static readonly VideoTracker Video2 = new(new Dictionary<double, string>
    {
        [0] = "A video dated 15th of September started. A young man is sat at a table in front of a green screen, there are card packs and dice piled up on the table, with two large piles of card packs in the middle. ",
        [12] = "The young man introduces himself as The Lucky Carder, he is recording a pack opening video for his YouTube channel. He is opening some Catch Monsters packs and yells about the possiblity of finding a shiny Transcendog card. ",
        [52] = "He spends some time opening packs and he comments on the rare card he finds, a rare Birchkin, not competitively viable but pretty. ",
        [62] = "He also comments that the cards are feeling extra silky smooth today and feel good in his hands. ",
        [92] = "He opens another pack, but is then interrupted by the sound of an old mobile phone ringtone. He stands up and walks out of frame."
    });

    public static readonly VideoTracker Video3 = new(new Dictionary<double, string>
    {
        [0] = "A video dated 2nd of October started.",
        [13] = "",
        [40] = "",
        [56] = "",
        [74] = "",
        [96] = "",
    });

    public static readonly VideoTracker Video4 = new(new Dictionary<double, string>
    {
        [0] = "A video dated 3rd of October started.",
        [43] = ""
    });

    public static readonly VideoTracker Video5 = new(new Dictionary<double, string>
    {
        [0] = "A video dated <ERROR> started. It is full of static"
    });

    public static readonly VideoTracker Video6 = new(new Dictionary<double, string>
    {
        [0] = "A video dated 3rd of October started.",
        [7] = "",
        [37] = "",
        [44] = "",
        [72] = "",
        [97] = ""
    });

    public static readonly VideoTracker Video7 = new(new Dictionary<double, string>
    {
        [0] = "A video dated 3rd of October started.",
        [3] = ""
    });

    public static readonly VideoTracker Video8 = new(new Dictionary<double, string>
    {
        [0] = "A video dated 10th of October started.",
        [3] = "",
        [13] = ""
    });
}