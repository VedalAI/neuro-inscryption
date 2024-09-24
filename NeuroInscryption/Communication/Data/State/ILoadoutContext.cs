using System.Collections.Generic;
using NeuroInscryption.Communication.Data.Describable;

namespace NeuroInscryption.Communication.Data.State;

public interface ILoadoutContext
{
    IEnumerable<SigilData> RelevantSigils { get; }
    IEnumerable<TraitData> RelevantTraits { get; }
}
