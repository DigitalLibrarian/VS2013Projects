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
            double forceIn, 
            int contactAreaIn,
            int thicknessIn,
            
            int yieldStrainIn, int fractureStrainIn, int strainAtYieldIn,
            out double deformDistance)
        {
            double force = (double)forceIn;
            double contactArea = (double)contactAreaIn / 100d;
            double thickness = (double)thicknessIn / 10000d;

            double strainAtYield = (double)strainAtYieldIn / 1000d;

            double strainConvert = 100000d;
            double fractureStrain = (double)fractureStrainIn / strainConvert;
            double yieldStrain = (double)yieldStrainIn / strainConvert;

            // TODO - decode and make the deform follow

            //Strain is measured as parts-per-100000, meaning that 100000 strain is 100% deformation
            double strain = (force / (thickness * contactArea)) * strainAtYield;

            deformDistance = strain / strainAtYield;

            var result = StressResult.Elastic;

            if (strain >= fractureStrain)
            {
                // broken through
                result = StressResult.Fracture;
            }
            else if (strain >= yieldStrain)
            {
                // inelastic (plastic) deformation
                result = StressResult.Plastic;
            }
            
            return result;
        }
    }
}
