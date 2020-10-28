using System.Collections.Generic;

namespace Almostengr.Christmaslightshow
{
    public interface IDataFile
    {
        string EffectFilePath {get;set;}
        string MusicFilePath {get;set;}

        IShowSummary LoadShowSummaryFromFile();
        IList<IEffectSequence> LoadEffectSequencesFromFile();
    }
}