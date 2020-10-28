using System.Device.Gpio;

namespace Almostengr.Christmaslightshow.ShowController
{
    public class RelayState {
        public PinValue RelayOn = PinValue.Low;
        public PinValue RelayOff = PinValue.High;
    }
}