using System;

namespace AlmostengrLightShow
{
    public class XLightsShowSummary
    {
        public string EffectName { get; set; }
        public int Occurrences { get; set; }
        public TimeSpan TotalTime { get; set; }

        public TimeSpan StringToTimeSpan(string time)
        {
            try
            {
                return TimeSpan.Parse(string.Concat("00:", time));
            }
            catch (FormatException)
            {
                return TimeSpan.Parse(time);
            }
        }
    }
}