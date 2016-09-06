using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public class TissueFactory : ITissueFactory
    {
        public ITissue Create(ITissueClass tissueClass, int bodySize)
        {
            int totalRelThick = tissueClass.TotalRelativeThickness;
            var layers = new List<ITissueLayer>();
            foreach (var tc in tissueClass.TissueLayers)
            {
                int tissueThick = (int)((double)bodySize * ((double)tc.RelativeThickness / (double)totalRelThick));
                layers.Add(new TissueLayer(tc, tc.Material, tissueThick));
            }
            return new Tissue(layers);
        }
    }
}
