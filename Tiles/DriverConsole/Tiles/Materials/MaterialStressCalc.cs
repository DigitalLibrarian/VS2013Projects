using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public static class MaterialStressCalc
    {
        public static double ShearCost1(IMaterial strikerMat, IMaterial strickenMat, double sharpness)
        {
            var factor = 1d;
            if (strikerMat.ShearYield > strickenMat.ShearYield)
            {
                factor = 0.0d;
            }
            return (((double)strickenMat.ShearYield) * 5000d) * factor
                / (((double)strikerMat.ShearYield) * sharpness * 10d);
        }

        public static double ShearCost2(IMaterial strikerMat, IMaterial strickenMat, double sharpness)
        {
            var factor = 1d;
            if (strikerMat.ShearFracture > strickenMat.ShearFracture)
            {
                factor = 0.0d;
            }
            return (((double)strickenMat.ShearFracture) * 5000d) * factor
                / (((double)strikerMat.ShearFracture) * sharpness * 10d);
        }

        public static double ShearCost3(IMaterial strikerMat, IMaterial strickenMat, double sharpness, double layerVolume)
        {
            return (((double)strickenMat.ShearFracture) * layerVolume * 5000d)
                / (((double)strikerMat.ShearFracture) * sharpness * 10d);
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
