using System;

namespace AlmostengrLightShow
{
    public class EffectSequence
    {
        public string EffectName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
        public string Element { get; set; }
        public string ElementType { get; set; }
        public string Files { get; set; }

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