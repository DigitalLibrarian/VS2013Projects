using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public static class MaterialStressCalc
    {
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
            return strain / 10000d;
        }

        public static double GetEdgedBreakThreshold(
            int contactAreaIn,
            int aYieldStrainIn, int aFractureStrainIn, int aStrainAtYieldIn,
            int bYieldStrainIn, int bFractureStrainIn, int bStrainAtYieldIn,
            double sharpnessMultiplier
            )
        {
            double A = FixContactArea(contactAreaIn);
            double rSY = (double)bYieldStrainIn / (double)aYieldStrainIn;
            double rSF = (double)bFractureStrainIn / (double)aFractureStrainIn;

            double Qa = 1;
            double Qw = 1;
            double S = sharpnessMultiplier;
            if (S == 0)
            {
                S = 1;
            }

            // M >= (rSY + (A+1)*rSF) * (10 + 2*Qa) / (S * Qw)
            return (rSY + ((double)A + 1d) * rSF)
                * (10 + (2 * Qa))
                / (S * Qw);
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
                yield, fracture, strainAtYield,
                strikerMat.SharpnessMultiplier
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
            double term3 = A+1d;

            return term1 * term2 * term3;
        }


        public static double ShearCost1(IMaterial strikerMat, IMaterial strickenMat, double sharpness)
        {
            if (strikerMat.ShearYield > strickenMat.ShearYield)
            {
                //return 0.0d;
            }
            return ((double)strickenMat.ShearYield * 5d)
                / (((double)strikerMat.ShearYield) * sharpness * 1000d);
        }

        public static double ShearCost2(IMaterial strikerMat, IMaterial strickenMat, double sharpness)
        {
            if (strikerMat.ShearFracture > strickenMat.ShearFracture)
            {
                //return 0.0d;
            }
            return ((((double)strickenMat.ShearFracture) * 5d)
                / ((double)strikerMat.ShearFracture) * sharpness);
        }

        public static double ShearCost3(IMaterial strikerMat, IMaterial strickenMat, double sharpness, double layerVolume)
        {
            return (((double)strickenMat.ShearFracture) * layerVolume * 5d)
                / ((((double)strikerMat.ShearFracture) * sharpness));
        }


        public static double ImpactCost1(IMaterial strickenMat, double layerVolume)
        {
            return (layerVolume * (double)(strickenMat.ImpactYield))
                / 100d / 500d / 10d;
        }

        public static double ImpactCost2(IMaterial strickenMat, double layerVolume)
        {
            return (layerVolume * ((double)(strickenMat.ImpactFracture) - (double)(strickenMat.ImpactYield)))
                / 100d / 500d / 10d;
        }

        public static double ImpactCost3(IMaterial strickenMat, double layerVolume)
        {
            return (layerVolume * ((double)(strickenMat.ImpactFracture) - (double)(strickenMat.ImpactYield)))
                / 100d / 500d / 10d;
        }
        
        public static double DefeatedLayerMomentumDeduction(IMaterial strikerMat, IMaterial strickenMat, double sharpness, double layerVolume)
        {
            var shearCost1 = ShearCost1(strikerMat, strickenMat, sharpness);
            var impactCost1 = ImpactCost1(strickenMat, layerVolume);
            return (System.Math.Max(
                shearCost1,
                impactCost1)) / 10d;
        }

        
        public static double ShearMomentumAfterUnbrokenRigidLayer(double momentum, IMaterial strickenMat)
        {
            return (((double)strickenMat.ShearStrainAtYield) * momentum) / 50000d;
        }

        public static double ImpactMomentumAfterUnbrokenRigidLayer(double momentum, IMaterial strickenMat)
        {
            return (((double)strickenMat.ImpactStrainAtYield) * momentum) / 50000d;
        }
        

    }
}
