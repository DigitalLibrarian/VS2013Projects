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
            // TODO - decode and make the deform follow
            //Strain is measured as parts-per-100000, meaning that 100000 strain is 100% deformation
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
