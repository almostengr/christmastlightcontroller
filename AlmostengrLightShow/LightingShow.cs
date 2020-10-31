using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

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

        private void CheckArguments(string[] arguments)
        {
            _showDetails = new ShowDetails();
            for (int i = 0; i < arguments.Length; i++)
            {
                switch (arguments[i])
                {
                    case "-s":
                        _showDetails.EffectsFileName = arguments[i + 1];
                        break;
                    case "-p":
                        _showDetails.ModelsFileName = arguments[i + 1];
                        break;
                }
            }
        }

        public string RunShow(string[] arguments)
        {
            CheckArguments(arguments);

            ProcessXlightsEffectFile effectsFile = new ProcessXlightsEffectFile(_logger, _showDetails.EffectsFileName);
            bool dataLoadSuccessful = effectsFile.LoadEffectFile();

            ProcessXlightsModelFile modelsFile = new ProcessXlightsModelFile(_logger, _showDetails.ModelsFileName);
            bool pinLoadSuccessful = modelsFile.LoadModelFile();

            if (dataLoadSuccessful == false || pinLoadSuccessful == false)
            {
                string message = "Error loading files occurred.";
                _logger.Log(message);
                return message;
            }

            _showDetails.XLightsEffectSequences = effectsFile.LoadEffectSequencesFromFile();
            _showDetails.XLightsShowSummary = effectsFile.LoadShowSummaryFromFile();
            _showDetails.XLightsModels = modelsFile.ProcessXLightModels();
            _showDetails.XLightsModelGroups = modelsFile.ProcessXLightsModelGroups();

            PlaySongWithLights();

            return "";
        }

        private void ControlLights()
        {
            _logger.Log(string.Concat("Now playing: ", _showDetails.EffectsFileName, " ", _showDetails.MusicFileName));

            using GpioController gpio = new GpioController();

            _logger.Log("Setting up pins");
            for (int pinNum = MinPinNumber; pinNum <= MaxPinNumber; pinNum++)
            {
#if RELEASE
                gpio.OpenPin(pinNum, PinMode.Output);
#endif
            }

            PinValue relayOff = PinValue.High;
            PinValue relayOn = PinValue.Low;

            Console.CancelKeyPress += (s, e) =>
            {
                _logger.Log("Emergency shutdown!");
                SwitchAllRelays(gpio, relayOff);
            };

            SwitchAllRelays(gpio, relayOff);

            TimeSpan showTime = new TimeSpan(0, 0, 0, 0, 0);
            int sleepInterval = 10;
            int showTimeInterval = 10;

            while (showTime < _showDetails.XLightsShowSummary.TotalTime)
            {
                var lights = _showDetails.XLightsEffectSequences.FindAll(x => x.StartTime == showTime || x.EndTime == showTime);

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
            _logger.Log(string.Concat("Toggle Element ", lightName, ", ", action.ToString()));

            var switchModel = _showDetails.XLightsModels.FindAll(e => e.ModelName == lightName);
            foreach (var model in switchModel)
            {
#if RELEASE
                control.Write(model.StartChannel, action);
#endif
            }

            var switchModelGroup = _showDetails.XLightsModelGroups.FindAll(e => e.ModelName == lightName);
            foreach (var modelGroup in switchModelGroup)
            {
                foreach (var channel in modelGroup.StartChannels)
                {
#if RELEASE
                    control.Write(channel, action);
#endif
                }
            }
        }

        private void PlaySongWithLights()
        {
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "/usr/bin/cvlc";
            ExternalProcess.StartInfo.ArgumentList.Add("--no-loop");
            ExternalProcess.StartInfo.ArgumentList.Add("--no-repeat");
            ExternalProcess.StartInfo.ArgumentList.Add("--play-and-exit");
            ExternalProcess.StartInfo.ArgumentList.Add("--intf");
            ExternalProcess.StartInfo.ArgumentList.Add("dummy");
            ExternalProcess.StartInfo.ArgumentList.Add(_showDetails.MusicFileName);
            ExternalProcess.Start();

            ControlLights();

            ExternalProcess.WaitForExit();
        }
    }
}