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

        int MinPinNumber { get; set; }
        int MaxPinNumber { get; set; }
        PinValue relayOn = PinValue.Low;
        PinValue relayOff = PinValue.High;

        public LightController(string v)
        {
            SongDataPath = v;
            EffectSequences = new List<EffectSequence>();
            _ShowSummary = new ShowSummary();
        }

        private void LoadEffectSequencesFromFile()
        {
            Console.WriteLine("Reading effect file");

            StreamReader reader = null;

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
                        lineData.StartTime = lineData.StringToTimeSpan(splitLine[1]);
                        lineData.EndTime = lineData.StringToTimeSpan(splitLine[2]);
                        lineData.Duration = lineData.StringToTimeSpan(splitLine[3]);
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
                Console.WriteLine("File was not found");
            }
            finally
            {
                reader.Close();
            }
        }

        public void LoadShowSummaryFromFile()
        {
            StreamReader reader = null;

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

                        Console.WriteLine("Song/show time: {0}", _ShowSummary.TotalTime.ToString());
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
                Console.WriteLine("File was not found");
            }
            finally
            {
                reader.Close();
            }
        }

        private void ShutOffRelays(GpioController controller)
        {
            for (int pinNum = MinPinNumber; pinNum <= MaxPinNumber; pinNum++)
            {
                controller.Write(pinNum, PinValue.High);
            }
        }

        public async Task ControlLights()
        {

            LoadEffectSequencesFromFile();

            if (EffectSequences.Count < 1)
            {
                throw new ApplicationException("Unable to load data file");
            }

            LoadShowSummaryFromFile();

            Console.WriteLine("Now playing: {0}", SongDataPath);
            TimeSpan showTime = new TimeSpan(0, 0, 0, 0, 0);

            using GpioController gpio = new GpioController();
            MinPinNumber = 2;
            MaxPinNumber = 27;

            for (int pinNum = MinPinNumber; pinNum <= MaxPinNumber; pinNum++)
            {
                Console.WriteLine("Setting up pin " + pinNum);
                gpio.OpenPin(pinNum, PinMode.Output);
            }

            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Emergency shutdown");
                ShutOffRelays(gpio);
            };

            ShutOffRelays(gpio);

            // TODO See if both intervals are needed after testing
            int sleepInterval = 10;
            int showTimeInterval = 10;
            while (showTime < _ShowSummary.TotalTime)
            {
                var lights = EffectSequences.Where(x => x.StartTime == showTime || x.EndTime == showTime);

                foreach (var light in lights)
                {
                    Console.WriteLine("{1}, Toggle Relay {0}", light.Element, showTime);

                    int pinNumber = PinLookup(light.Element);
                    if (light.StartTime == showTime)
                    {
                        gpio.Write(pinNumber, relayOn);
                    }
                    else
                    {
                        gpio.Write(pinNumber, relayOff);
                    }
                }

                showTime = showTime.Add(TimeSpan.FromMilliseconds(showTimeInterval));
                Thread.Sleep(sleepInterval);
            }

            ShutOffRelays(gpio);
        }

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
                    // Console.WriteLine("Not sure. Random number.");
                    return new Random().Next(02, 27);
                    break;
            }
        }
    }
}