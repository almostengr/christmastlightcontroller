using System.Device.Gpio;

namespace Almostengr.Christmaslightshow
{
    public class RelayState {
        public PinValue RelayOn = PinValue.Low;
        public PinValue RelayOff = PinValue.High;
    }
}