using System;
using System.Diagnostics;
using Almostengr.Christmaslightshow.ShowController.ShowController;

namespace Almostengr.Christmaslightshow.ShowConductor
{
    public class Conductor
    {
        string ShowDataFiles { get; set; }
        string ControllerPath { get; set; }

        public Conductor()
        {
#if DEBUG
            ShowDataFiles = "/home/almostengineer/christmaslightshow/xlights/showData/";
            ControllerPath = "/home/almostengineer/christmaslightshow/lightshow/";
#else
            ShowDataFiles = "/home/pi/";
            ControllerPath = "/home/almostengineer/christmaslightshow/";
#endif
        }

        public void CheckClock()
        {
            TimeSpan showStartTime = new TimeSpan(17, 30, 00);
            TimeSpan showEndTime = new TimeSpan(22, 00, 00);

            TimeSpan sleepStartTime = showEndTime;
            TimeSpan sleepEndTime = new TimeSpan(7, 00, 00);

            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            if (currentTime >= showStartTime && currentTime <= showEndTime)
            {
                RunShowMode();
            }
            else if (currentTime > sleepStartTime || currentTime <= sleepEndTime)
            {
                RunNightMode();
            }
            else
            {
                RunDayMode();
            }
        }

        private void PlaySongWithLights(string sequenceFile, string musicFile)
        {
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "/usr/bin/cvlc";
            ExternalProcess.StartInfo.ArgumentList.Add("--play-and-exit");
            ExternalProcess.StartInfo.ArgumentList.Add("--intf");
            ExternalProcess.StartInfo.ArgumentList.Add("dummy");
            ExternalProcess.StartInfo.ArgumentList.Add(musicFile); // song name
            ExternalProcess.Start();

            LightController controller = new LightController(ShowDataFiles + "20thCenturyFox.csv");
            controller.ControlLights();

            ExternalProcess.WaitForExit();
        }

        private void RunDayMode()
        {
            PlaySongWithLights(ShowDataFiles + "ChristmasMusicDay.csv", ShowDataFiles + "ChristmasMusic.mp3");
        }

        private void RunNightMode()
        {
            PlaySongWithLights(ShowDataFiles + "ChristmasMusicNight.csv", ShowDataFiles + "ChristmasMusic.mp3");
        }

        private void RunShowMode()
        {
            PlaySongWithLights(ShowDataFiles + "20thCenturyFox.csv", ShowDataFiles + "20thCenturyFox.mp3");
        }
    }
}