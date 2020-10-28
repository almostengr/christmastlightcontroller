using System;

namespace Almostengr.Christmaslightshow
{
    public interface ILightShow
    {
        string TimeSpanToString(TimeSpan time);

        TimeSpan StringToTimeSpan(string time);
    }
}