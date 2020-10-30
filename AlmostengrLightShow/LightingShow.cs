using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AlmostengrLightShow
{
    public class LightingShow
    {
        private ConsoleLogger _logger { get; set; }
        private ShowDetails _showDetails { get; set; }
        private const int MinPinNumber = 2;
        private const int MaxPinNumber = 25;

        public LightingShow(ConsoleLogger logger)
        {
            _logger = logger;
        }

        private void SwitchAllRelays(GpioController controller, PinValue relayState)
        {
            for (int pinNum = MinPinNumber; pinNum <= MaxPinNumber; pinNum++)
            {
                controller.Write(pinNum, relayState);
            }
        }

        public void CheckArguments(string[] arguments)
        {
            _showDetails = new ShowDetails();
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == "-s")
                {
                    _showDetails.DataFileName = arguments[i + 1];
                    break;
                }
            }
        }

        public async Task RunShow(string[] arguments)
        {
            CheckArguments(arguments);

            ProcessDataFile dataFile = new ProcessDataFile(_logger);
            dataFile.FilePath = _showDetails.DataFileName;
            bool loadSuccessful = dataFile.LoadFile();

            if (loadSuccessful == false)
            {
                throw new ApplicationException("Unable to load data file");
            }

            _showDetails.EffectSequences = dataFile.LoadEffectSequencesFromFile();
            _showDetails.ShowSummary = dataFile.LoadShowSummaryFromFile();

            PlaySongWithLights();
        }

        public void ControlLights()
        {
            _logger.Log(string.Concat("Now playing: {0}", _showDetails.DataFileName));
            TimeSpan showTime = new TimeSpan(0, 0, 0, 0, 0);

            using GpioController gpio = new GpioController();

            PinValue relayOff = PinValue.High;
            PinValue relayOn = PinValue.Low;

            for (int pinNum = MinPinNumber; pinNum <= MaxPinNumber; pinNum++)
            {
                _logger.Log("Setting up pin " + pinNum);
                gpio.OpenPin(pinNum, PinMode.Output);
            }

            Console.CancelKeyPress += (s, e) =>
            {
                _logger.Log("Emergency shutdown");
                SwitchAllRelays(gpio, relayOff);
            };

            SwitchAllRelays(gpio, relayOff);

            int sleepInterval = 10;
            int showTimeInterval = 10;

            while (showTime < _showDetails.ShowSummary.TotalTime)
            {
                var lights = _showDetails.EffectSequences.FindAll(x => x.StartTime == showTime || x.EndTime == showTime);

                foreach (var light in lights)
                {
                    if (light.StartTime == showTime)
                    {
                        PinLookup(gpio, light.Element, relayOn);
                    }
                    else
                    {
                        PinLookup(gpio, light.Element, relayOff);
                    }
                }

                showTime = showTime.Add(TimeSpan.FromMilliseconds(showTimeInterval));
                Thread.Sleep(sleepInterval);
            }
        }

        private void PinLookup(GpioController control, string lightName, PinValue action)
        {
            List<int> pinNumbers = new List<int>();
            switch (lightName)
            {
                case "RedTree1":
                    pinNumbers.Add(14);
                    break;

                case "WhiteTree1":
                    pinNumbers.Add(18);
                    break;

                default:
                    pinNumbers.Add(new Random().Next(MinPinNumber, MaxPinNumber));
                    break;
            }

            foreach (var pinNumber in pinNumbers)
            {
                _logger.Log(string.Concat("Toggle Relay ", lightName, " ", action.ToString()));
                control.Write(pinNumber, action);
            }
        }

        private void PlaySongWithLights()
        {
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "/usr/bin/cvlc";
            ExternalProcess.StartInfo.ArgumentList.Add("--play-and-exit");
            ExternalProcess.StartInfo.ArgumentList.Add("--intf");
            ExternalProcess.StartInfo.ArgumentList.Add("dummy");
            ExternalProcess.StartInfo.ArgumentList.Add(_showDetails.MusicFileName); // song name
            ExternalProcess.Start();

            ControlLights();

            ExternalProcess.WaitForExit();
        }
    }
}