using System;

namespace Almostengr.Christmaslightshow
{
    public interface IEffectSequence : ILightShow
    {
        string EffectName { get; set; }
        TimeSpan StartTime { get; set; }
        TimeSpan EndTime { get; set; }
        TimeSpan Duration { get; set; }
        int? DurationInt { get; set; }
        string Description { get; set; }
        string Element { get; set; }
        string ElementType { get; set; }
        string Files { get; set; }
    }
}