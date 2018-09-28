using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class MaterialStressCalc : IMaterialStressCalc
    {
        private static readonly double MinRelativeToughness = 0.01d;
        //  1. A small momentum cost to start denting the layer material, if the weapon has a higher shear yield than the layer.
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
        
        // 2. A small momentum cost to start cutting the layer material, if the weapon has a higher shear fracture than the layer.
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

        // 3. A large momentum cost to cut through the volume of the layer material, using the ratio of weapon to layer shear fractures and the weapon's sharpness.
        public double ShearCost3(IMaterial strikerMat, IMaterial strickenMat, double sharpness, double layerVolume)
        {
            var factor = 1d;
            if (strikerMat.ShearFracture > strickenMat.ShearFracture)
            {
                factor = (double)strickenMat.ShearFracture / (double)strikerMat.ShearFracture;
                factor = System.Math.Max(MinRelativeToughness, factor);
            }
            else if (strikerMat.ShearFracture == strickenMat.ShearFracture)
            {
                factor = MinRelativeToughness;
            }

            return (((double)strickenMat.ShearFracture) * layerVolume * 5000d * factor)
                / (((double)strikerMat.ShearFracture) * sharpness * 10d);
        }

        // 2. A momentum cost to dent the layer volume, using the layer's impact yield.
        public double ImpactCost1(IMaterial strickenMat, double layerVolume)
        {
            return (layerVolume * (double)(strickenMat.ImpactYield))
                / 100d / 500d / 10d;
        }

        // 3. A momentum cost to initiate fracture in the layer volume, using the difference between the layer's impact fracture and impact yield.
        public double ImpactCost2(IMaterial strickenMat, double layerVolume)
        {
            return (layerVolume * ((double)(strickenMat.ImpactFracture) - (double)(strickenMat.ImpactYield)))
                / 100d / 500d / 10d;
        }

        // 4. A momentum cost to complete fracture in the layer volume, which is the same as step 3.
        public double ImpactCost3(IMaterial strickenMat, double layerVolume)
        {
            return (layerVolume * ((double)(strickenMat.ImpactFracture) - (double)(strickenMat.ImpactYield)))
                / 100d / 500d / 10d;
        }
    }
}
