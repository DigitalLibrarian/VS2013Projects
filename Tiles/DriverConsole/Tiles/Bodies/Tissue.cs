using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public class Tissue : ITissue
    {
        public IEnumerable<ITissueLayer> TissueLayers { get; private set; }
        public Tissue(IList<ITissueLayer> layers)
        {
            TissueLayers = layers;
        }

        public double TotalThickness
        {
            get { return TissueLayers.Select(tl => tl.Thickness).Sum(); }
        }
    }
}
