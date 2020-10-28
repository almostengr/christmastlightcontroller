using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace Almostengr.Christmaslightshow
{
    public class RelayControl : IRelayControl
    {
        public GpioController gpioController { get; set; }

        int MinPinNumber {get; set;}
        int MaxPinNumber {get;set;}

        public RelayControl()
        {
            // GpioController gpioController = new GpioController();
            MinPinNumber = 2;
            MaxPinNumber = 27;

            // foreach (var pin in GpioPins.){
            for (int pinNum = MinPinNumber; pinNum <= MaxPinNumber; pinNum++)
            {
                Console.WriteLine(DateTime.Now + ": Setting up pins");
                // gpioController.OpenPin(pinNum, PinMode.Output);
                // gpioController.Write(pinNum, PinValue.Low);
            }
        }

        public async Task TurnOnLightAsync(string lightName, TimeSpan duration)
        {
            var on = PinValue.Low;
            var off = PinValue.High;

            // Console.WriteLine(DateTime.Now + ": Relay {0} for {1}", lightName, duration.ToString());
            // gpioController.Write(Enum.TryParse(GpioPins, lightName, true), RelayState.RelayOn);
            // gpioController.Write()

            // await Task.Delay((int)duration.TotalMilliseconds);
            // Console.WriteLine(DateTime.Now + ": Turning off {0}", lightName);

            int pinNumber;

            // switch (lightName)
            // {
            //     case "RedTree1":
            //         pinNumber = 14;
            //         break;

            //     case "WhiteTree1":
            //         pinNumber = 18;
            //         break;

            //     default:
            //         Console.WriteLine(DateTime.Now + ": Not sure. Random number.");
            //         pinNumber = new Random().Next(02,27);
            //         break;
            // }

            pinNumber = PinLookup(lightName);

            Console.WriteLine(DateTime.Now + ": Relay On {0}", pinNumber);
            // gpioController.Write(pinNumber, PinValue.Low);
            // await Task.Delay(duration);
            // gpioController.Write(pinNumber, PinValue.High);
        }

        private int PinLookup(string lightName){

            switch (lightName)
            {
                case "RedTree1":
                    return 14;
                    break;

                case "WhiteTree1":
                    return 18;
                    break;

                default:
                    Console.WriteLine(DateTime.Now + ": Not sure. Random number.");
                    return new Random().Next(02,27);
                    break;
            }
        }

        public async Task TurnOnLightsAsync(IList<EffectSequence> lights)
        {
            foreach (var light in lights)
            {
                TurnOnLightAsync(light.Element, light.Duration);
            }
        }
        public async Task TurnOnLightsAsync(IEnumerable<IEffectSequence> lights)
        {
            foreach (var light in lights)
            {
                TurnOnLightAsync(light.Element, light.Duration);
            }
        }

        public async Task TurnOffLightsAsync(IEnumerable<IEffectSequence> lights)
        {
            foreach (var light in lights)
            {
                Console.WriteLine(DateTime.Now + ": Relay off {0} for {1}", light.Element);
                int pinNumber;
                pinNumber = PinLookup(light.Element);
                // gpioController.Write(pinNumber, PinValue.High);
                Console.WriteLine(DateTime.Now + ": On {0}", pinNumber);
            }
        }
    }
}