using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface ISingleLayerStrikeTester
    {
        MaterialStrikeResult StrikeTest(
            StressMode mode,
            IMaterial strikerMat, double strikerSharpness, double strikerContactArea,
            double momentum, double maxPenetration, double penetrationLeft,
            IMaterial strickenMat, double strickenThickness, double strickenVolume,
            double strickenContactArea, bool implementWasSmall);
    }
}
