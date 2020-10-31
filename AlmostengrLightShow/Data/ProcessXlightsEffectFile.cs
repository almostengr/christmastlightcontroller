using System;
using System.Collections.Generic;
using System.IO;

namespace AlmostengrLightShow
{
    public class ProcessXlightsEffectFile : Processor
    {
        private ConsoleLogger _logger;
        private string _filePath { get; set; }
        private char _columnSeparator = ',';
        private List<string> _fileContents { get; set; }

        public ProcessXlightsEffectFile(ConsoleLogger logger, string filePath)
        {
            _logger = logger;
            _fileContents = new List<String>();
            _filePath = filePath;
        }

        public bool LoadEffectFile()
        {
            _logger.Log("Reading effect file");

            bool loadSuccess = false;
            StreamReader reader = null;

            try
            {
                reader = new StreamReader(File.OpenRead(_filePath));
                while (!reader.EndOfStream)
                {
                    _fileContents.Add(reader.ReadLine());
                }

                loadSuccess = true;

                reader.Close();
                _logger.Log("Done reading effect file");
            }
            catch (FileNotFoundException)
            {
                _logger.Log("File was not found");
            }
            catch (Exception ex)
            {
                _logger.Log(string.Concat("Error occurred. ", ex.Message));
            }

            return loadSuccess;
        }

        public List<XLightsEffect> LoadEffectSequencesFromFile()
        {
            List<XLightsEffect> effectSequences = new List<XLightsEffect>();

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

                XLightsEffect effectSequence = new XLightsEffect();
                var splitLine = line.Split(_columnSeparator);

                effectSequence.EffectName = StanitizeCsvValue(splitLine[0]);
                effectSequence.StartTime = effectSequence.StringToTimeSpan(splitLine[1]);
                effectSequence.EndTime = effectSequence.StringToTimeSpan(splitLine[2]);
                effectSequence.Duration = effectSequence.StringToTimeSpan(splitLine[3]);
                effectSequence.Description = StanitizeCsvValue(splitLine[4]);
                effectSequence.Element = StanitizeCsvValue(splitLine[5]);
                effectSequence.ElementType = StanitizeCsvValue(splitLine[6]);
                effectSequence.Files = StanitizeCsvValue(splitLine[7]);

                effectSequences.Add(effectSequence);
            }

            return effectSequences;
        }

        public XLightsShowSummary LoadShowSummaryFromFile()
        {
            XLightsShowSummary showSummary = new XLightsShowSummary();
            bool summaryData = false;

            foreach (var line in _fileContents)
            {
                if (summaryData == true)
                {
                    var splitLine = line.Split(_columnSeparator);

                    showSummary.EffectName = StanitizeCsvValue(splitLine[0]);
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