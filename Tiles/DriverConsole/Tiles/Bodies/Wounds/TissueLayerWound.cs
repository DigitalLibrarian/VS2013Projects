using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Wounds
{
    public interface ITissueLayerWound
    {
        ITissueLayer Layer { get; }
        int Pain { get; }
    }

    public class TissueLayerWound : ITissueLayerWound
    {
        public ITissueLayer Layer { get; set; }
        public int Pain { get; set; }
    }
}
