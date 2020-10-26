using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace christmaslightshow
{
    class Program
    {
        private static LightShowSummaryData _showSummaryData;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // DateTime timeclock = new DateTime();
            TimeSpan timeclock = new TimeSpan();
            int counter = 0;
            Console.WriteLine("Starting the show!");

            var fileData = LoadCsvFile("/home/almostengineer/Downloads/countdown.csv");

            // _showSummaryData.TotalTime = string.Concat("00:", _showSummaryData.TotalTime, "0000");

            Console.WriteLine(_showSummaryData.TotalTime);
            TimeSpan timeConverted = TimeSpan.FromMilliseconds(counter);

            // use for playing audio file via VLC   
            // vlc --intf dummy ../Desktop/untitled.mp3
            // cvlc --play-and-exit --intf dummy ../Desktop/untitled.mp3
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "/usr/bin/cvlc"; // --play-and-exit --intf dummy /home/almostengineer/Desktop/untitled.mp3";
                                                                  // ExternalProcess.StartInfo.ArgumentList = {"--play-and-exit",
                                                                  //   "--intf dummy",
                                                                  //   "/home/almostengineer/Desktop/untitled.mp3"};
            ExternalProcess.StartInfo.ArgumentList.Add("--play-and-exit");
            ExternalProcess.StartInfo.ArgumentList.Add("--intf");
            ExternalProcess.StartInfo.ArgumentList.Add("dummy");
            ExternalProcess.StartInfo.ArgumentList.Add("/home/almostengineer/Desktop/untitled.mp3");

            // ExternalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            ExternalProcess.Start();
            // ExternalProcess.WaitForExit();

            // while (string.Concat("00:", _showSummaryData.TotalTime, "0000") != timeConverted.ToString())
            while (timeConverted < _showSummaryData.TotalTime)
            {
                if (counter == 0)
                {
                    ExternalProcess.Start();
                }

                // Console.WriteLine(TimeSpan.FromMilliseconds(counter));
                // Console.WriteLine(timeConverted);
                // timeclock.Add(TimeSpan.FromMilliseconds(counter));

                // if (timeConverted.ToString().EndsWith("0000") == false)
                //     Console.WriteLine(timeConverted.ToString());

                // fileData.FindAll(timeConverted.ToString());
                // List<LightShowData> lightsOn = fileData.FindAll(x => x.StartTime == timeConverted);
                // if (lightsOn.Count > 0){
                //     TurnLightsOn(lightsOn);
                // }

                // // Console.WriteLine("matching lines: {0}", lightsOn.Count);
 
                List<LightShowData> lightsOn = fileData.FindAll(x => x.StartTime == timeConverted);
                foreach (var light in lightsOn)
                {
                    var tasks = new Task[] {};
                    TurnOnLightAsync(light.Element, light.Duration);
                    // tasks.Start();
                }

                counter++;
                timeConverted = TimeSpan.FromMilliseconds(counter);
                Thread.Sleep(1);

                // Thread.Sleep(1000);
            }

            Console.WriteLine("End of show!");
        }

        private static async Task TurnLightsOnAsync(List<LightShowData> lightList)
        {
            // var lightsOn = fileData.FindAll(x => x.StartTime == timeConverted);

            // Console.WriteLine("matching lines: {0}", lightsOn.Count);

            foreach (var light in lightList)
            {
                // TurnOnLight(light.Element, light.Duration);

                Console.WriteLine("Turning on {0} for {1}", light.EffectName, light.Duration.ToString());
                // Thread.Sleep(light.Duration.Milliseconds);
            }
        }

        private static async Task TurnOnLightAsync(string lightName, TimeSpan duration)
        {
            Console.WriteLine("Turning on {0} for {1}", lightName, duration.ToString());
            Thread.Sleep((int)duration.TotalMilliseconds);
            // Console.WriteLine("Duration: {0}, {1}", duration.ToString(), duration.TotalMilliseconds);
            Console.WriteLine("Turning off {0}", lightName);
        }

        public static List<LightShowData> LoadCsvFile(string filePath)
        {
            Console.WriteLine("Reading data file");

            var reader = new StreamReader(File.OpenRead(filePath));
            List<LightShowData> searchList = new List<LightShowData>();

            try
            {
                // List<string> searchList = new List<string>();
                bool summaryData = false;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    // searchList.Add(line);

                    if (summaryData == true)
                    {
                        var lineData = new LightShowSummaryData();
                        var splitLine = line.Split(",");

                        lineData.EffectName = splitLine[0];
                        lineData.Occurrences = Int32.Parse(splitLine[1]);

                        try
                        {
                            lineData.TotalTime = TimeSpan.Parse(splitLine[2]);
                        }
                        catch (FormatException)
                        {
                            lineData.TotalTime = TimeSpan.Parse(string.Concat("00:", splitLine[2]));
                        }

                        _showSummaryData = lineData;

                        summaryData = false;

                        Console.WriteLine("Max time: {0}", _showSummaryData.TotalTime.ToString());
                    }

                    if (line.StartsWith("Effect Name,Occurences,TotalTime"))
                    {
                        summaryData = true;
                    }

                    if (line.StartsWith("\"On") && line.EndsWith(","))
                    {
                        var lineData = new LightShowData();
                        var splitLine = line.Split(",");

                        lineData.EffectName = splitLine[0];
                        // lineData.StartTime = splitLine[1];
                        // lineData.EndTime = splitLine[2];
                        // lineData.Duration = splitLine[3];

                        lineData.StartTime = TimeSpan.Parse(string.Concat("00:", splitLine[1]));
                        lineData.EndTime = TimeSpan.Parse(string.Concat("00:", splitLine[2]));
                        lineData.Duration = TimeSpan.Parse(string.Concat("00:", splitLine[3]));
                        lineData.Description = splitLine[4];
                        lineData.Element = splitLine[5];
                        lineData.ElementType = splitLine[6];
                        lineData.Files = splitLine[7];

                        searchList.Add(lineData);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("file was not found");
            }

            reader.Close();
            return searchList;
        }
    }
}
