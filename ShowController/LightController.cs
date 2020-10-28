using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Almostengr.Christmaslightshow.ShowController.ShowController
{
    public class LightController
    {
        private string SongDataPath { get; }
        private IList<EffectSequence> EffectSequences { get; set; }
        private char _columnSeparator = ',';
        private ShowSummary _ShowSummary { get; set; }
        private GpioController gpioController { get; set; }

        int MinPinNumber { get; set; }
        int MaxPinNumber { get; set; }

        public LightController(string v)
        {
            SongDataPath = v;
            EffectSequences = new List<EffectSequence>();
            _ShowSummary = new ShowSummary();
        }

        // private IList<EffectSequence> LoadEffectSequencesFromFile()
        private void LoadEffectSequencesFromFile()
        {
            Console.WriteLine(DateTime.Now + ": Reading effect file");

            StreamReader reader = null;
            // IList<EffectSequence> effectSequences = new List<EffectSequence>();
            // char _columnSeparator = ',';

            try
            {
                reader = new StreamReader(File.OpenRead(SongDataPath));

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line.StartsWith("\"On") && line.EndsWith(_columnSeparator))
                    {
                        var lineData = new EffectSequence();
                        var splitLine = line.Split(_columnSeparator);

                        lineData.EffectName = splitLine[0];
                        lineData.StartTime = lineData.StringToTimeSpan(splitLine[1]); // ) TimeSpan.Parse(string.Concat("00:", splitLine[1]));
                        lineData.EndTime = lineData.StringToTimeSpan(splitLine[2]); //  TimeSpan.Parse(string.Concat("00:", splitLine[2]));
                        lineData.Duration = lineData.StringToTimeSpan(splitLine[3]); // TimeSpan.Parse(string.Concat("00:", splitLine[3]));
                        lineData.Description = splitLine[4];
                        lineData.Element = splitLine[5];
                        lineData.ElementType = splitLine[6];
                        lineData.Files = splitLine[7];

                        EffectSequences.Add(lineData);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(DateTime.Now + ": File was not found");
            }
            finally
            {
                reader.Close();
            }

            // return effectSequences;
        }

        public void LoadShowSummaryFromFile()
        {
            StreamReader reader = null;
            // ShowSummary showSummary = new ShowSummary();

            try
            {
                reader = new StreamReader(File.OpenRead(SongDataPath));
                bool summaryData = false;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (summaryData == true)
                    {
                        var splitLine = line.Split(_columnSeparator);

                        _ShowSummary.EffectName = splitLine[0];
                        _ShowSummary.Occurrences = Int32.Parse(splitLine[1]);
                        _ShowSummary.TotalTime = _ShowSummary.StringToTimeSpan(splitLine[2]);

                        summaryData = false;

                        Console.WriteLine(DateTime.Now + ": Song/show time: {0}", _ShowSummary.TotalTime.ToString());
                        break;
                    }
                    else if (line.StartsWith("Effect Name,Occurences,TotalTime"))
                    {
                        summaryData = true;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(DateTime.Now + ": File was not found");
            }
            finally
            {
                reader.Close();
            }

            // return showSummary;
        }


        public async Task ControlLights()
        {

            LoadEffectSequencesFromFile();

            if (EffectSequences.Count < 1)
            {
                throw new ApplicationException("Unable to load data file");
            }

            LoadShowSummaryFromFile();

            Console.WriteLine(DateTime.Now + ": Now playing: {0}", SongDataPath);
            TimeSpan showTime = new TimeSpan(0, 0, 0, 0, 0);

            // RelayControl relayControl = new RelayControl();
// /            int index = 0;
            // var sequences = EffectSequences.OrderBy(x => x.StartTime);
            // var sequences = EffectSequences.Where(x => x.StartTime == )

            using GpioController gpio = new GpioController();
            MinPinNumber = 2;
            MaxPinNumber = 27;

            // foreach (var pin in GpioPins.){
            for (int pinNum = MinPinNumber; pinNum <= MaxPinNumber; pinNum++)
            {
                Console.WriteLine("Setting up pin " + pinNum);
                gpio.OpenPin(pinNum, PinMode.Output);
            }

            int sleepInterval = 10;
            var on = PinValue.Low;
            var off = PinValue.High;
            while (showTime < _ShowSummary.TotalTime)
            {
                var lights = EffectSequences.Where(x => x.StartTime == showTime || x.EndTime == showTime);
                

                // if (lightsOn.Count() > 0){
                //     relayControl.TurnOnLightsAsync(lightsOn);
                // }
                foreach (var light in lights)
                {
                    // int gpioPin = PinLookup(light.Element)
                    // Console.WriteLine("Relay on {0}", lightOn.)
                    
                    Console.WriteLine("{1}, Toggle Relay {0}", light.Element, showTime);
                    
                    int pinNumber = PinLookup(light.Element);
                    if (light.StartTime == showTime) {
                        gpio.Write(pinNumber, on);
                    }
                    else {
                        gpio.Write(pinNumber, off);
                    }
                }

                // if (lightsOff.Count() > 0)
                //     relayControl.TurnOffLightsAsync(lightsOff);

                showTime = showTime.Add(TimeSpan.FromMilliseconds(sleepInterval));

                // await Task.Delay(1);
                // Task.Delay(1);
                Thread.Sleep(sleepInterval);
            }

            // while (showTime < _ShowSummary.TotalTime)
            // {
            //     // EffectSequence effectSequence = effectSequences.ElementAt(index);
            //     var sequence = EffectSequences.ElementAt(index);
            //     if (sequence.StartTime == showTime)
            //     {
            //         Console.WriteLine(DateTime.Now + ": On " + sequence.Element);
            //     }

            //     showTime = showTime.Add(TimeSpan.FromMilliseconds(10));

            //     Task.Delay(10);
            // }
        }

        // private async Task ToggleLightOn(string lightElement, GpioController gpioController){
        //     int pinNumber = PinLookup(lightElement);
        //     gp
        // }

        // private async Task TurnOnLightsAsync(IList)


        // public void RelayControl()
        // {
        //     // GpioController gpioController = new GpioController();
        //     MinPinNumber = 2;
        //     MaxPinNumber = 27;

        //     // foreach (var pin in GpioPins.){
        //     for (int pinNum = MinPinNumber; pinNum <= MaxPinNumber; pinNum++)
        //     {
        //         Console.WriteLine(DateTime.Now + ": Setting up pins");
        //         // gpioController.OpenPin(pinNum, PinMode.Output);
        //         // gpioController.Write(pinNum, PinValue.Low);
        //     }
        // }

        // public async Task TurnOnLightAsync(string lightName, TimeSpan duration)
        // {
        //     var on = PinValue.Low;
        //     var off = PinValue.High;

        //     // Console.WriteLine(DateTime.Now + ": Relay {0} for {1}", lightName, duration.ToString());
        //     // gpioController.Write(Enum.TryParse(GpioPins, lightName, true), RelayState.RelayOn);
        //     // gpioController.Write()

        //     // await Task.Delay((int)duration.TotalMilliseconds);
        //     // Console.WriteLine(DateTime.Now + ": Turning off {0}", lightName);

        //     int pinNumber;

        //     // switch (lightName)
        //     // {
        //     //     case "RedTree1":
        //     //         pinNumber = 14;
        //     //         break;

        //     //     case "WhiteTree1":
        //     //         pinNumber = 18;
        //     //         break;

        //     //     default:
        //     //         Console.WriteLine(DateTime.Now + ": Not sure. Random number.");
        //     //         pinNumber = new Random().Next(02,27);
        //     //         break;
        //     // }

        //     pinNumber = PinLookup(lightName);

        //     Console.WriteLine(DateTime.Now + ": Relay On {0}", pinNumber);
        //     // gpioController.Write(pinNumber, PinValue.Low);
        //     // await Task.Delay(duration);
        //     // gpioController.Write(pinNumber, PinValue.High);
        // }

        // private async Task SwitchPin (string lightName, PinValue lightState)
        // {
         
        //     switch (lightName)
        //     {
        //         case "RedTree1":
        //             return 14;
        //             break;

        //         case "WhiteTree1":
        //             return 18;
        //             break;

        //         default:
        //             Console.WriteLine(DateTime.Now + ": Not sure. Random number.");
        //             return new Random().Next(02, 27);
        //             break;
        //     }   
        // }

        private int PinLookup(string lightName)
        {
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
                    return new Random().Next(02, 27);
                    break;
            }
        }

        // public async Task TurnOnLightsAsync(IList<EffectSequence> lights)
        // {
        //     foreach (var light in lights)
        //     {
        //         TurnOnLightAsync(light.Element, light.Duration);
        //     }
        // }
        // public async Task TurnOnLightsAsync(IEnumerable<IEffectSequence> lights)
        // {
        //     foreach (var light in lights)
        //     {
        //         TurnOnLightAsync(light.Element, light.Duration);
        //     }
        // }

        // public async Task TurnOffLightsAsync(IEnumerable<IEffectSequence> lights)
        // {
        //     foreach (var light in lights)
        //     {
        //         Console.WriteLine(DateTime.Now + ": Relay off {0} for {1}", light.Element);
        //         int pinNumber;
        //         pinNumber = PinLookup(light.Element);
        //         // gpioController.Write(pinNumber, PinValue.High);
        //         Console.WriteLine(DateTime.Now + ": On {0}", pinNumber);
        //     }
        // }
    }
}