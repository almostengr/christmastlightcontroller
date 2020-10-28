using System;

namespace Almostengr.Christmaslightshow.ShowController
{
    public class ShowSummary
    {
        public string EffectName { get; set; }
        public int Occurrences { get; set; }
        public TimeSpan TotalTime { get; set; }

        // public int TotalTimeInt {get; set;}

        // public void SetTotalTimeInt()
        // {
        //     TotalTimeInt = (int)TotalTime.TotalMilliseconds;
        // }

        public string TimeSpanToString(TimeSpan time)
        {
            return time.ToString();
        }
        
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