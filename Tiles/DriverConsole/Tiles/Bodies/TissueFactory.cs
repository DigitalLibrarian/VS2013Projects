using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public class TissueFactory : ITissueFactory
    {
        public ITissue Create(ITissueClass tissueClass, double partSize, double strength)
        {
            double storedFat = 500000d;
            var partThick = System.Math.Pow(partSize * 10000d, 0.333d);
            int totalRelThick = tissueClass.TotalRelativeThickness;
            var layers = new List<ITissueLayer>();
            foreach (var tc in tissueClass.TissueLayers)
            {
                var fractionTotal = (double)totalRelThick;
                var tlFact = (double)tc.RelativeThickness;
                var mlpf = tlFact;

                if (tc.ThickensOnStrength)
                {
                    mlpf = strength * tlFact / 1000d;
                }

                if (tc.ThickensOnEnergyStorage)
                {
                    mlpf = storedFat * tlFact / 2500 / 100;
                }

                var tissueThick = partThick * mlpf / fractionTotal;
                var tissueVol = partSize * mlpf / fractionTotal;
                tissueThick = System.Math.Max(1d, tissueThick);
                tissueVol = System.Math.Max(1d, tissueVol);

                var damage = new DamageVector();
                layers.Add(new TissueLayer(tc, tissueThick, tissueVol, damage));
            }
            return new Tissue(layers);
        }
    }
}
