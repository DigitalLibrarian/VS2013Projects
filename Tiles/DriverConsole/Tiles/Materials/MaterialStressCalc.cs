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
            int momentum, int contactArea, 
            int yieldForce, int fractureForce, int strainAtYield,
            out int deformDistance)
        {
            deformDistance = strainAtYield * momentum;
            var result = StressResult.Elastic;

            if (momentum >= fractureForce)
            {
                // broken through
                result = StressResult.Fracture;
            }
            else if (momentum >= yieldForce)
            {
                // inelastic (plastic) deformation
                result = StressResult.Plastic;
            }
            
            return result;
        }
    }
}
