using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public static class MaterialStressCalc
    {
        // TODO - items or creatures without defined attacks get default blunt attack with area = (size)^2/3

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
            double stress = (force / (thickness * contactArea)) * strainAtYield;

            deformDistance = stress / strainAtYield;

            var result = StressResult.Elastic;

            if (stress >= fractureStrain)
            {
                // broken through
                result = StressResult.Fracture;
            }
            else if (stress >= yieldStrain)
            {
                // inelastic (plastic) deformation
                result = StressResult.Plastic;
            }
            
            return result;
        }



        public static bool EdgedStress(
            double forceIn, int contactAreaIn, int thicknessIn,
            int aYieldStrainIn, int aFractureStrainIn, int aStrainAtYieldIn,
            int bYieldStrainIn, int bFractureStrainIn, int bStrainAtYieldIn
            )
        {
            double yieldRatio = aYieldStrainIn / bYieldStrainIn;
            double fractureRatio = aFractureStrainIn / bFractureStrainIn;

            var requiredMomentum = 10d * (yieldRatio + (contactAreaIn + 1) * fractureRatio)/ 10000000d;
            return forceIn >= requiredMomentum;
        }

        public static bool BluntStress(
            int weaponSize,
            double forceIn, int contactAreaIn, int thicknessIn,
            int aYieldStrainIn, int aFractureStrainIn, int aStrainAtYieldIn,
            int bYieldStrainIn, int bFractureStrainIn, int bStrainAtYieldIn
            )
        {
            double yieldRatio = aYieldStrainIn / bYieldStrainIn;
            double fractureRatio = aFractureStrainIn / bFractureStrainIn;

            //Blunt attacks can be entirely deflected by armor if weapon's IMPACT_YIELD is especially low relative to armor's density:
            // 2 * Sw * IYw < A * Da,


            // Otherwise
            // M >= (2*IF - IY) * (2 + 0.4*Qa) * A,

            var requiredMomentum = (((fractureRatio) - yieldRatio) * 2.4 * (contactAreaIn));

            return forceIn >= requiredMomentum;
        }
    }
}
