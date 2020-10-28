using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Almostengr.Christmaslightshow.ShowController.ShowController;

namespace Almostengr.ShowConductor
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

            // if (currentTime >= showStartTime && currentTime <= showEndTime)
            // {
            //     RunShowMode();
            // }
            // else if (currentTime > sleepStartTime || currentTime <= sleepEndTime)
            // {
            //     RunNightMode();
            // }
            // else
            // {
            //     RunDayMode();
            // }

            RunNightMode();
        }

        private async Task RunNightMode()
        {
            // sign, columns, and radio on
            // var music = PlayMusic(ShowDataFiles + "20thCenturyFox.mp3");
            // var lights = ShowLights(ShowDataFiles + "20thCenturyFox.csv");

            DoSong(ShowDataFiles + "20thCenturyFox.csv", ShowDataFiles + "20thCenturyFox.mp3");

            // await music;
            // await lights;
        }

        private void DoSong(string sequenceFile, string musicFile)
        {
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "/usr/bin/cvlc"; // --play-and-exit --intf dummy /home/almostengineer/Desktop/untitled.mp3";
            ExternalProcess.StartInfo.ArgumentList.Add("--play-and-exit");
            ExternalProcess.StartInfo.ArgumentList.Add("--intf");
            ExternalProcess.StartInfo.ArgumentList.Add("dummy");
            ExternalProcess.StartInfo.ArgumentList.Add(musicFile); // song name

            // Process ExternalProcess2 = new Process();
            // Console.WriteLine(ControllerPath + "ShowController");
            // Console.WriteLine(sequenceFile);
            // ExternalProcess2.StartInfo.FileName = ControllerPath + "ShowController";
            // ExternalProcess2.StartInfo.ArgumentList.Add(sequenceFile); // song name

            ExternalProcess.Start();
            // ExternalProcess2.Start();

            
            LightController controller = new LightController(ShowDataFiles + "20thCenturyFox.csv");
            controller.ControlLights();


            ExternalProcess.WaitForExit();
        }

        private void RunDayMode()
        {
            // sign and radio on
            throw new NotImplementedException();
        }

        private void RunShowMode()
        {
            throw new NotImplementedException();
        }

        private async Task PlayMusic(string musicFile)
        {
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "/usr/bin/cvlc"; // --play-and-exit --intf dummy /home/almostengineer/Desktop/untitled.mp3";
            ExternalProcess.StartInfo.ArgumentList.Add("--play-and-exit");
            ExternalProcess.StartInfo.ArgumentList.Add("--intf");
            ExternalProcess.StartInfo.ArgumentList.Add("dummy");
            ExternalProcess.StartInfo.ArgumentList.Add(musicFile); // song name
            ExternalProcess.Start();
            // ExternalProcess.WaitForExit();
        }

        private async Task ShowLights(string sequenceFile)
        {
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = string.Concat(ControllerPath, "ShowController");
            ExternalProcess.StartInfo.ArgumentList.Add(sequenceFile); // song name
            ExternalProcess.Start();
            ExternalProcess.WaitForExit();
        }
    }
}