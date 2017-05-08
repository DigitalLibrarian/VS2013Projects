using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class SingleLayerStrikeTester : ISingleLayerStrikeTester
    {
        IMaterialStrikeResultBuilder Builder { get; set; }

        public SingleLayerStrikeTester(IMaterialStrikeResultBuilder builder)
        {
            Builder = builder;
        }

        public MaterialStrikeResult StrikeTest(
            StressMode mode,
            IMaterial strikerMat, double strikerSharpness, double strikerContactArea,
            double momentum, double penetrationLeft,
            IMaterial strickenMat, double strickenThickness, double strickenVolume,
            double strickenContactArea)
        {
            Builder.Clear();

            Builder.SetStressMode(mode);
            Builder.SetStrikerMaterial(strikerMat);
            Builder.SetStrikerSharpness(strikerSharpness);
            Builder.SetStrickenMaterial(strickenMat);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetLayerVolume(strickenVolume);
            Builder.SetLayerThickness(strickenThickness);
            Builder.SetRemainingPenetration(penetrationLeft);

            Builder.SetStrikerContactArea(strikerContactArea);
            Builder.SetStrickenContactArea(strickenContactArea);

            return Builder.Build();
        }
    }
}
