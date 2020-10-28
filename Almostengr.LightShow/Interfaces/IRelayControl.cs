using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almostengr.Christmaslightshow
{
    public interface IRelayControl
    {
        Task TurnOnLightsAsync (IList<EffectSequence> lights);
    }
}