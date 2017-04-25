using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;
using Tiles.Math;

namespace Tiles.Bodies
{
    public class TissueLayer : ITissueLayer
    {
        public ITissueLayerClass Class { get; private set; }
        public IMaterial Material { get; private set; }
        public double Thickness { get; private set; }
        public double Volume { get; private set; }

        public TissueLayer(ITissueLayerClass layerClass, IMaterial material, double thickness, double volume)
        {
            Class = layerClass;
            Material = material;
            Thickness = thickness;
            Volume = volume;
        }
    }
}
