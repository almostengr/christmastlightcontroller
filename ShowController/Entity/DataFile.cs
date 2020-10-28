using System;
using System.Collections.Generic;
using System.IO;

namespace Almostengr.Christmaslightshow.ShowController
{
    public class DataFile
    {
        public string EffectFilePath { get; set; }
        public string MusicFilePath { get; set; }
        private ShowSummary _ShowSummary {get; set;}

        private char _columnSeparator = ',';

        public DataFile(string filePath)
        {
            EffectFilePath = filePath;
            MusicFilePath = string.Concat(filePath.Substring(0, filePath.Length - 3), "mp3");
        }

        public void LoadShowSummaryFromFile()
        {
            StreamReader reader = null;
            // ShowSummary showSummary = new ShowSummary();

            try
            {
                reader = new StreamReader(File.OpenRead(EffectFilePath));
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

        public IList<EffectSequence> LoadEffectSequencesFromFile()
        {
            Console.WriteLine(DateTime.Now + ": Reading effect file");

            StreamReader reader = null;
            IList<EffectSequence> effectSequences = new List<EffectSequence>();

            try
            {
                reader = new StreamReader(File.OpenRead(EffectFilePath));

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

                        effectSequences.Add(lineData);
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

            return effectSequences;
        }

    }
}