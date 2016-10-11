using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public class TissueFactory : ITissueFactory
    {
        public ITissue Create(ITissueClass tissueClass, double partSize)
        {
            var partThick = System.Math.Pow(partSize * 10000d, 0.333d);
            int totalRelThick = tissueClass.TotalRelativeThickness;
            var layers = new List<ITissueLayer>();
            foreach (var tc in tissueClass.TissueLayers)
            {
                var tlFact = (double)tc.RelativeThickness / (double)totalRelThick;
                var tissueThick = System.Math.Floor(partThick * tlFact);
                tissueThick = System.Math.Ceiling(tissueThick);
                tissueThick = System.Math.Max(1d, tissueThick);

                var tissueVol = partSize * tlFact;
                if (tc.ThickensOnStrength)
                {
                    tissueVol = partSize * 1250d * tlFact / 1000d;
                }
                tissueVol = System.Math.Ceiling(tissueVol);
                tissueVol = System.Math.Max(1d, tissueVol);

                layers.Add(new TissueLayer(tc, tc.Material, tissueThick, tissueVol));
            }
            return new Tissue(layers);
        }
    }
}
