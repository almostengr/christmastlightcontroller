using System;
using System.Collections.Generic;
using System.IO;

namespace Almostengr.Christmaslightshow
{
    public class DataFile : IDataFile
    {
        public string EffectFilePath { get; set; }
        public string MusicFilePath { get; set; }

        private char _columnSeparator = ',';

        public DataFile(string filePath)
        {
            EffectFilePath = filePath;
            MusicFilePath = string.Concat(filePath.Substring(0, filePath.Length - 3), "mp3");
        }

        public IShowSummary LoadShowSummaryFromFile()
        {
            StreamReader reader = null;
            IShowSummary showSummary = new ShowSummary();

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

                        showSummary.EffectName = splitLine[0];
                        showSummary.Occurrences = Int32.Parse(splitLine[1]);
                        showSummary.TotalTime = showSummary.StringToTimeSpan(splitLine[2]);

                        summaryData = false;

                        Console.WriteLine(DateTime.Now + ": Song/show time: {0}", showSummary.TotalTime.ToString());
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

            return showSummary;
        }

        public IList<IEffectSequence> LoadEffectSequencesFromFile()
        {
            Console.WriteLine(DateTime.Now + ": Reading effect file");

            StreamReader reader = null;
            IList<IEffectSequence> effectSequences = new List<IEffectSequence>();

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