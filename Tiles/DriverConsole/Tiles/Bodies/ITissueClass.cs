using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public interface ITissueClass
    {
        IEnumerable<ITissueLayerClass> TissueLayers { get; set; }
        int TotalRelativeThickness { get; }
    }
}
