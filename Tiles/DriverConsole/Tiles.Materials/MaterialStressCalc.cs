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
        public double ShearCost1(double strickenYield, double strikerYield, double sharpness)
        {
            var factor = 1d;
            if (strikerYield > strickenYield)
            {
                factor = strickenYield / strikerYield;
                factor = System.Math.Max(MinRelativeToughness, factor);
            }
            return ((strickenYield) * 5000d) * factor
                / ((strikerYield) * sharpness * 10d);
        }

        // 2. A small momentum cost to start cutting the layer material, if the weapon has a higher shear fracture than the layer.
        public double ShearCost2(double strickenFracture, double strikerFracture, double sharpness)
        {
            var factor = 1d;
            if (strikerFracture > strickenFracture)
            {
                factor = strickenFracture / strikerFracture;
                factor = System.Math.Max(MinRelativeToughness, factor);
            }

            return ((strickenFracture) * 5000d) * factor
                / ((strikerFracture) * sharpness * 10d);
        }

        // 3. A large momentum cost to cut through the volume of the layer material, using the ratio of weapon to layer shear fractures and the weapon's sharpness.
        public double ShearCost3(double strickenFracture, double strikerFracture, double sharpness, double layerVolume)
        {
            var factor = 1d;
            if (strikerFracture > strickenFracture)
            {
                factor = strickenFracture / strikerFracture;
                factor = System.Math.Max(MinRelativeToughness, factor);
            }
            else if (strikerFracture == strickenFracture)
            {
                factor = MinRelativeToughness;
            }

            return ((strickenFracture) * layerVolume * 5000d * factor)
                / ((strikerFracture) * sharpness * 10d);
        }

        // 2. A momentum cost to dent the layer volume, using the layer's impact yield.
        public double BluntCost1(double strickenYield, double layerVolume)
        {
            return (layerVolume * (double)(strickenYield))
                / 100d / 500d / 10d;
        }

        // 3. A momentum cost to initiate fracture in the layer volume, using the difference between the layer's impact fracture and impact yield.
        public double BluntCost2(double strickenYield, double strickenFracture, double layerVolume)
        {
            return (layerVolume * ((double)(strickenFracture) - (double)(strickenYield)))
                / 100d / 500d / 10d;
        }

        // 4. A momentum cost to complete fracture in the layer volume, which is the same as step 3.
        public double BluntCost3(double strickenYield, double strickenFracture, double layerVolume)
        {
            return BluntCost2(strickenYield, strickenFracture, layerVolume);
        }
    }
}
