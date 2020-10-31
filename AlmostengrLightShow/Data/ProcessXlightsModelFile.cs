using System;
using System.Collections.Generic;
using System.IO;

namespace AlmostengrLightShow
{
    public class ProcessXlightsModelFile : Processor
    {
        private ConsoleLogger _logger;
        private string _filePath { get; }
        private char _columnSeparator = ',';
        private List<string> _fileContents { get; set; }

        public ProcessXlightsModelFile(ConsoleLogger logger, string filePath)
        {
            _logger = logger;
            _fileContents = new List<String>();
            _filePath = filePath;
        }

        public bool LoadModelFile()
        {
            _logger.Log("Reading model file");

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
                _logger.Log("Done reading model file");
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

        public List<XLightsModelGroup> ProcessXLightsModelGroups()
        {
            List<XLightsModelGroup> xLightsModelGroups = new List<XLightsModelGroup>();

            foreach (var line in _fileContents.FindAll(f => f.Contains("ModelGroup")))
            {
                XLightsModelGroup xLightsModelGroup = new XLightsModelGroup();
                var splitLine = line.Split(_columnSeparator);

                xLightsModelGroup.ModelName = StanitizeCsvValue(splitLine[0]);
                xLightsModelGroup.Description = StanitizeCsvValue(splitLine[2]);

                xLightsModelGroups.Add(xLightsModelGroup);
            }

            return xLightsModelGroups;
        }

        public List<XLightsModel> ProcessXLightModels()
        {
            List<XLightsModel> xLightsModels = new List<XLightsModel>();

            foreach (var line in _fileContents.FindAll(f => f.Contains("ModelGroup") == false))
            {
                if (line.StartsWith("Model Name"))
                {
                    continue;
                }
                else if (line.Equals(""))
                {
                    break;
                }

                XLightsModel xLightsModel = new XLightsModel();
                var splitLine = line.Split(_columnSeparator);

                xLightsModel.ModelName = StanitizeCsvValue(splitLine[0]);
                xLightsModel.StartChannel = Int32.Parse(splitLine[13]);

                xLightsModels.Add(xLightsModel);
            }

            return xLightsModels;
        }

        public List<XLightsModelGroup> MapModelsToModelGroups(List<XLightsModelGroup> xLightsModelGroups, List<XLightsModel> xLightsModels)
        {
            foreach (var xLightsModel in xLightsModels)
            {
                foreach (var xLightsModelGroup in xLightsModelGroups)
                {
                    if (xLightsModelGroup.Description.Contains(xLightsModel.ModelName))
                    {
                        xLightsModelGroup.StartChannels.Add(xLightsModel.StartChannel);
                    }
                }
            } // end foreach

            return xLightsModelGroups;
        }
    }
}