using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Almostengr.Christmaslightshow.ShowController
{
    public class LightShow
    {

        // public void RunShow()
        // {
        //     using GpioController gpio = new GpioController();


        // }

        public async Task RunLightShow()
        {
            Console.WriteLine(DateTime.Now + ": Start the show!");

            var songs = LoadSongs();
            RelayControl relayControl = new RelayControl();

            foreach (var song in songs)
            {
                IDataFile dataFile = new DataFile(song);

                ShowSummary showSummary = dataFile.LoadShowSummaryFromFile();
                IList<EffectSequence> effectSequence = dataFile.LoadEffectSequencesFromFile();

                Console.WriteLine(DateTime.Now + ": Now playing: {0}", song);
                PlayMusicAsync(dataFile);
                await DoLightsAsync(showSummary, effectSequence);

                // var lights = Task.Run(() => DoLightsAsync(showSummary, effectSequence));
                // var music = Task.Run(() => PlayMusicAsync(dataFile));
                // await lights; 
                // await music;

                // await Task.Delay((int)showSummary.TotalTime.TotalMilliseconds);
            }
            Console.WriteLine(DateTime.Now + ": End of show!");
        }

        public async Task PlayMusicAsync(IDataFile dataFile)
        {
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "/usr/bin/cvlc"; // --play-and-exit --intf dummy /home/almostengineer/Desktop/untitled.mp3";
            ExternalProcess.StartInfo.ArgumentList.Add("--play-and-exit");
            ExternalProcess.StartInfo.ArgumentList.Add("--intf");
            ExternalProcess.StartInfo.ArgumentList.Add("dummy");
            ExternalProcess.StartInfo.ArgumentList.Add(dataFile.MusicFilePath); // song name
            ExternalProcess.Start();
            ExternalProcess.WaitForExit();
        }

        // public async Task DoLightsAsync(ShowSummary showSummary, IList<EffectSequence> effectSequences)
        public async Task DoLightsAsync(ShowSummary showSummary, IList<EffectSequence> effectSequences)
        {
            // int counter = 0;
            TimeSpan showTime = new TimeSpan(0, 0, 0, 0, 0);
            RelayControl relayControl = new RelayControl();

            // while (showTime < showSummary.TotalTime)
            // {
            //     var lightsOn = effectSequences.Where(x => x.StartTime == showTime);

            //     if (lightsOn.Count() > 0){
            //         relayControl.TurnOnLightsAsync(lightsOn);
            //     }

            //     var lightsOff = effectSequences.Where(x => x.EndTime == showTime);

            //     if (lightsOff.Count() > 0)
            //         relayControl.TurnOffLightsAsync(lightsOff);

            //     showTime = showTime.Add(TimeSpan.FromMilliseconds(1));

            //     await Task.Delay(1);
            //     // Task.Delay(1);
            // }

            int index = 0;
            var sequences = effectSequences.OrderBy(x => x.StartTime);
            while (showTime < showSummary.TotalTime)
            {
                // EffectSequence effectSequence = effectSequences.ElementAt(index);
                var sequence = sequences.ElementAt(index);
                if (sequence.StartTime == showTime){
                    Console.WriteLine(DateTime.Now + ": On " + sequence.Element);
                }

                showTime = showTime.Add(TimeSpan.FromMilliseconds(10));

                await Task.Delay(10);
            }
        }

        public IList<string> LoadSongs()
        {
            IList<string> songs = new List<string>();
            songs.Add("/home/almostengineer/christmaslightshow/xlights/showData/20thCenturyFox.csv");
            // songs.Add("/home/almostengineer/christmaslightshow/xlights/showData/01-ChristmasTimeIsPartyTime.csv");
            // songs.Add("/home/pi/20thCenturyFox.csv");
            // songs.Add("/home/pi/01-ChristmasTimeIsPartyTime.csv");
            return songs;
        }

        public void IsItShowtime()
        {
            Console.WriteLine(DateTime.Now + ": Checking show time");
            // if (DateTime.Now.Hour >= 18 && DateTime.Now.Hour <= 22)
            if (true)
            {
                RunLightShow();
            }
            // else
            // {
            //     RunRadio();
            // }
        }
    }
}