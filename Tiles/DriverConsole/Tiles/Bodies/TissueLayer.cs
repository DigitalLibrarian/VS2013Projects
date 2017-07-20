using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies.Injuries;
using Tiles.Materials;
using Tiles.Math;

namespace Tiles.Bodies
{
    public class TissueLayer : ITissueLayer
    {
        public ITissueLayerClass Class { get; private set; }
        public IMaterial Material { get { return Class.Material; } }
        public double Thickness { get; private set; }
        public double Volume { get; private set; }

        public string Name { get { return Material.Name; } }

        public TissueLayer(ITissueLayerClass layerClass, double thickness, double volume)
        {
            Class = layerClass;
            Thickness = thickness;
            Volume = volume;
        }

        public bool IsPulped()
        {
            return false;
        }

        public bool IsVascular()
        {
            return Class.VascularRating > 0;
        }
    }
}
