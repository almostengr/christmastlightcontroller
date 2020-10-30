using System;
using System.Collections.Generic;
using System.IO;

namespace AlmostengrLightShow
{
    public class ProcessDataFile
    {
        private ConsoleLogger _logger;
        public string FilePath { get; set; }
        private char _columnSeparator = ',';
        private List<string> _fileContents { get; set; }

        public ProcessDataFile(ConsoleLogger logger)
        {
            _logger = logger;
            _fileContents = new List<String>();
        }

        public bool LoadFile()
        {
            _logger.Log("Reading effect file");

            bool loadSuccess = false;
            StreamReader reader = null;

            try
            {
                reader = new StreamReader(File.OpenRead(FilePath));
                while (!reader.EndOfStream)
                {
                    _fileContents.Add(reader.ReadLine());
                }
                loadSuccess = true;
            }
            catch (FileNotFoundException)
            {
                _logger.Log("File was not found");
            }
            finally
            {
                reader.Close();
            }

            _logger.Log("Done reading effect file");

            return loadSuccess;
        }

        public List<EffectSequence> LoadEffectSequencesFromFile()
        {
            List<EffectSequence> effectSequences = new List<EffectSequence>();

            foreach (var line in _fileContents)
            {
                if (line.Contains("Effect Count"))
                {
                    break;
                }
                else if (line.Contains("Effect"))
                {
                    continue;
                }

                EffectSequence effectSequence = new EffectSequence();
                var splitLine = line.Split(_columnSeparator);

                effectSequence.EffectName = splitLine[0];
                effectSequence.StartTime = effectSequence.StringToTimeSpan(splitLine[1]);
                effectSequence.EndTime = effectSequence.StringToTimeSpan(splitLine[2]);
                effectSequence.Duration = effectSequence.StringToTimeSpan(splitLine[3]);
                effectSequence.Description = splitLine[4];
                effectSequence.Element = splitLine[5];
                effectSequence.ElementType = splitLine[6];
                effectSequence.Files = splitLine[7];

                effectSequences.Add(effectSequence);
            }

            return effectSequences;
        }

        public ShowSummary LoadShowSummaryFromFile()
        {
            ShowSummary showSummary = new ShowSummary();
            bool summaryData = false;

            foreach (var line in _fileContents)
            {
                if (summaryData == true)
                {
                    var splitLine = line.Split(_columnSeparator);

                    showSummary.EffectName = splitLine[0];
                    showSummary.Occurrences = Int32.Parse(splitLine[1]);
                    showSummary.TotalTime = showSummary.StringToTimeSpan(splitLine[2]);

                    _logger.Log(string.Concat("Song/show time: ", showSummary.TotalTime.ToString()));
                    break;
                }
                else if (line.StartsWith("Effect Name,Occurences,TotalTime"))
                {
                    summaryData = true;
                }
            }

            return showSummary;
        }
    }
}