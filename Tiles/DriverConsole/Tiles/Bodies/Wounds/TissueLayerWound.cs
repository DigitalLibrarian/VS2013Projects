using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Wounds
{
    class TissueLayerWound
    {
        public int WoundArea { get; set; }

        public double ContactAreaRatio { get; set; }
        public double PenetrationRatio { get; set; }

        public int Pain { get; set; }
    }
}
