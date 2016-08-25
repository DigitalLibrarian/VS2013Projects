using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public class TissueFactory : ITissueFactory
    {
        public ITissue Create(ITissueClass tissueClass)
        {
            var layers = new List<ITissueLayer>();
            foreach (var tc in tissueClass.TissueLayers)
            {
                layers.Add(new TissueLayer(
                    tc.Material,
                    tc.RelativeThickness
                    ));
            }
            return new Tissue(layers);
        }
    }
}
