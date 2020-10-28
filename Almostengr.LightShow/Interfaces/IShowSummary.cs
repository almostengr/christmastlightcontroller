using System;

namespace Almostengr.Christmaslightshow
{
    public interface IShowSummary : ILightShow
    {
        string EffectName { get; set; }
        int Occurrences { get; set; }
        TimeSpan TotalTime { get; set; }
        int TotalTimeInt { get; set; }

        void SetTotalTimeInt();
    }
}