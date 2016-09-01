using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public class TissueLayer : ITissueLayer
    {
        public IMaterial Material { get; private set; }
        public int Thickness { get; private set; }

        public TissueLayer(IMaterial material, int thickness)
        {
            Material = material;
            Thickness = thickness;
        }
        
        public bool CanBeBruised { get; set; }
        public bool CanBeTorn { get; set; }
        public bool CanBePunctured { get; set; }
    }
}
