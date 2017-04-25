using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class MaterialStressCalc : Tiles.Materials.IMaterialStressCalc
    {
        private static readonly double MinRelativeToughness = 0.01d;
        public double ShearCost1(IMaterial strikerMat, IMaterial strickenMat, double sharpness)
        {
            var factor = 1d;
            if (strikerMat.ShearYield > strickenMat.ShearYield)
            {
                factor = (double)strickenMat.ShearYield / (double)strikerMat.ShearYield;
                factor = System.Math.Max(MinRelativeToughness, factor);
            }
            return (((double)strickenMat.ShearYield) * 5000d) * factor
                / (((double)strikerMat.ShearYield) * sharpness * 10d);
        }

        public double ShearCost2(IMaterial strikerMat, IMaterial strickenMat, double sharpness)
        {
            var factor = 1d;
            if (strikerMat.ShearFracture > strickenMat.ShearFracture)
            {
                factor = (double)strickenMat.ShearFracture / (double)strikerMat.ShearFracture;
                factor = System.Math.Max(MinRelativeToughness, factor);
            }
            return (((double)strickenMat.ShearFracture) * 5000d) * factor
                / (((double)strikerMat.ShearFracture) * sharpness * 10d);
        }

        public double ShearCost3(IMaterial strikerMat, IMaterial strickenMat, double sharpness, double layerVolume)
        {

            var factor = 1d;
            if (strikerMat.ShearFracture > strickenMat.ShearFracture)
            {
                factor = (double)strickenMat.ShearFracture / (double)strikerMat.ShearFracture;
                factor = System.Math.Max(MinRelativeToughness, factor);
            }

            return (((double)strickenMat.ShearFracture) * layerVolume * 5000d * factor)
                / (((double)strikerMat.ShearFracture) * sharpness * 10d);
        }


        public double ImpactCost1(IMaterial strickenMat, double layerVolume)
        {
            return (layerVolume * (double)(strickenMat.ImpactYield))
                / 100d / 500d / 10d;
        }

        public double ImpactCost2(IMaterial strickenMat, double layerVolume)
        {
            return (layerVolume * ((double)(strickenMat.ImpactFracture) - (double)(strickenMat.ImpactYield)))
                / 100d / 500d / 10d;
        }

        public double ImpactCost3(IMaterial strickenMat, double layerVolume)
        {
            return (layerVolume * ((double)(strickenMat.ImpactFracture) - (double)(strickenMat.ImpactYield)))
                / 100d / 500d / 10d;
        }
        
        public double CalculateStrain(double strainAtYield, double stressAtYield, double currentStress)
        {
            var say = strainAtYield;
            var youngs = stressAtYield / say;
            return (currentStress / youngs) * 100000d;
        }
    }
}
