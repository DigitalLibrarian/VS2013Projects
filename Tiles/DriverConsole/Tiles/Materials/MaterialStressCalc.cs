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



        public static double CalcStrain(double forceIn, int contactAreaIn, int strainAtYieldIn)
        {
            double force = (double)forceIn;
            double contactArea = FixContactArea(contactAreaIn);
            double strainAtYield =  ((double)strainAtYieldIn) / 100000d;

            //Strain is measured as parts-per-100000, meaning that 100000 strain is 100% deformation
            return ((force * strainAtYield) / contactArea );
        }

        public static StressResult StressLayer(
            double forceIn, 
            int contactAreaIn,
            int thicknessIn,
            
            int yieldStrainIn, int fractureStrainIn, int strainAtYieldIn,
            out double deformRatio)
        {
            double strainAtYield = (double)strainAtYieldIn;
            double strainConvert = 100000d;
            double fractureStrain = (double)fractureStrainIn / strainConvert;
            double yieldStrain = (double)yieldStrainIn / strainConvert;


            var stress = CalcStrain(forceIn, contactAreaIn, strainAtYieldIn);
            var result = StressResult.Elastic;

            if (stress >= fractureStrain)
            {
                // broken through
                result = StressResult.Fracture;
                deformRatio = 1d;
            }
            else if (stress >= yieldStrain)
            {
                // inelastic (plastic) deformation
                result = StressResult.Plastic;
                deformRatio = 1d;
            }
            else
            {
                deformRatio = stress * (strainAtYield / 100000d);
            }
            
            return result;
        }




        public static bool EdgedStress(
            double forceIn, int contactAreaIn, int thicknessIn,
            int aYieldStrainIn, int aFractureStrainIn, int aStrainAtYieldIn,
            int bYieldStrainIn, int bFractureStrainIn, int bStrainAtYieldIn
            )
        {
            var requiredMomentum = GetEdgedBreakThreshold(contactAreaIn, aYieldStrainIn, aFractureStrainIn, aStrainAtYieldIn, bYieldStrainIn, bFractureStrainIn, bStrainAtYieldIn);
            return forceIn >= requiredMomentum;
        }

        public static bool BluntStress(
            double forceIn, int contactAreaIn, int thicknessIn,
            int aYieldStrainIn, int aFractureStrainIn, int aStrainAtYieldIn,
            int bYieldStrainIn, int bFractureStrainIn, int bStrainAtYieldIn)
        {
            double yieldRatio = (double) aYieldStrainIn / (double) bYieldStrainIn;
            double fractureRatio = (double)aFractureStrainIn / (double)bFractureStrainIn;

            //Blunt attacks can be entirely deflected by armor if weapon's IMPACT_YIELD is especially low relative to armor's density:
            // 2 * Sw * IYw < A * Da,


            // Otherwise
            // M >= (2*IF - IY) * (2 + 0.4*Qa) * A,

            var requiredMomentum = ((2d*fractureRatio) - yieldRatio) * 2.4 * contactAreaIn;

            return forceIn >= requiredMomentum;
        }


        #region Not Stupid
        private static double FixContactArea(int contactArea)
        {
            return (double)contactArea / 100d;
        }

        private static double ToMpa(int strain)
        {
            return ToMpa((double)strain);
        }
        private static double ToMpa(double strain)
        {
            return strain / 1000000d;
        }

        public static double GetEdgedBreakThreshold(
            int contactAreaIn,
            int aYieldStrainIn, int aFractureStrainIn, int aStrainAtYieldIn,
            int bYieldStrainIn, int bFractureStrainIn, int bStrainAtYieldIn
            )
        {
            double contactArea = FixContactArea(contactAreaIn);
            double yieldRatio = (double)aYieldStrainIn / (double)bYieldStrainIn;
            double fractureRatio = (double)aFractureStrainIn / (double)bFractureStrainIn;

            yieldRatio = 1 / yieldRatio;
            fractureRatio = 1 / fractureRatio;


            double qa = 1;
            double qw = 1;
            double s = 1;


            return (yieldRatio + ((double)contactArea + 1d) * fractureRatio)
                * (10 * 2 * qa)
                / (s * qw);
        }

        public static double GetEdgedBreakThreshold(int contactArea, IMaterial strikerMat, IMaterial strickenMat)
        {
            int wYield, wFracture, wStrainAtYield;
            strikerMat.GetModeProperties(StressMode.Edge,
                out wYield, out wFracture, out wStrainAtYield);

            int yield, fracture, strainAtYield;
            strickenMat.GetModeProperties(StressMode.Edge,
                out yield, out fracture, out strainAtYield);

            return GetEdgedBreakThreshold(
                contactArea,
                wYield, wFracture, wStrainAtYield,
                yield, fracture, strainAtYield
                );
        }

        public static double GetBluntBreakThreshold(int contactArea, IMaterial strickenMat)
        {
            int yield, fracture, strainAtYield;
            strickenMat.GetModeProperties(StressMode.Blunt,
                out yield, out fracture, out strainAtYield);

            var IF = ToMpa(fracture);   // layer fracture
            var IY = ToMpa(yield);      // layer yield

            var A = FixContactArea(contactArea);
            double Qa = 1d; // quality multiplier for armor

            // M >= (2*IF - IY) * (2 + 0.4*Qa) * A,

            double term1 = (2d * IF) - IY;
            if (term1 < 0)
            {
                term1 = 1d;
            }
            double term2 = 2d + 0.4d * Qa;
            double term3 = A;

            return term1 * term2 * term3;
        }

        #endregion
    }
}
