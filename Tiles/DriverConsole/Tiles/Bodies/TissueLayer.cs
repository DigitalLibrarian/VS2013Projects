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
        public int RelativeThickness { get; private set; }

        public TissueLayer(IMaterial material, int relativeThickness)
        {
            Material = material;
            RelativeThickness = relativeThickness;
        }

        // TODO - need some way to track damage

    }
}
