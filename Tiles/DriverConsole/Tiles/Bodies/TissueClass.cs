using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public class TissueClass : ITissueClass
    {
        public TissueClass(IEnumerable<ITissueLayerClass> tLClasses)
        {
            TissueLayers = tLClasses;
        }
        public IEnumerable<ITissueLayerClass> TissueLayers { get; set; }
    }
}
