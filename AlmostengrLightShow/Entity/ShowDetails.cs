using System.Collections.Generic;

namespace AlmostengrLightShow
{
    public class ShowDetails
    {
        public string EffectsFileName { get; set; }
        public string ModelsFileName { get; set; }
        public string MusicFileName { get => EffectsFileNameToMusicFileName(); }
        public List<XLightsEffect> XLightsEffectSequences { get; set; }
        public XLightsShowSummary XLightsShowSummary { get; set; }
        public List<XLightsModel> XLightsModels { get; set; }
        public List<XLightsModelGroup> XLightsModelGroups { get; set; }

        private string EffectsFileNameToMusicFileName()
        {
            var name = EffectsFileName.Replace(".csv", ".mp3");
            return name;
        }
    }
}