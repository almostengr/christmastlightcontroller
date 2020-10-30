using System.Collections.Generic;

namespace AlmostengrLightShow
{
    public class ShowDetails
    {
        public string DataFileName { get; set; }
        public string MusicFileName { get => DataFileNameToMusicFileName(); }
        public List<EffectSequence> EffectSequences { get; set; }
        public ShowSummary ShowSummary { get; set; }

        private string DataFileNameToMusicFileName()
        {
            var name = DataFileName.Replace(".csv", ".mp3");
            return name;
        }
    }
}