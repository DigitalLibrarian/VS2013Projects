using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public static class MaterialStressCalc
    {
        public static StressResult StressLayer(
            int momPerTissueVolume, int yieldForce, int fractureForce, int strainAtYield,
            out int deformDistance)
        {
            deformDistance = strainAtYield * momPerTissueVolume;
            var result = StressResult.Elastic;

            if (momPerTissueVolume >= fractureForce)
            {
                // broken through
                result = StressResult.Fracture;
            }
            else if (momPerTissueVolume >= yieldForce)
            {
                // inelastic (plastic) deformation
                result = StressResult.Plastic;
            }
            
            return result;
        }
    }
}
