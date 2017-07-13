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

        public IDamageVector Damage { get; private set; }

        public TissueLayer(ITissueLayerClass layerClass, double thickness, double volume, IDamageVector damage)
        {
            Class = layerClass;
            Thickness = thickness;
            Volume = volume;

            Damage = damage;
        }

        // affected surface area
        public double WoundArea { get; set; }

        // ratio of layer surface area to wounded area
        public double WoundAreaRatio { get; set; }

        // ratio of the penetrated depth
        public double PenetrationRatio { get; set; }

        public bool IsPulped()
        {
            return Damage.IsPulped();
        }

        public bool IsVascular()
        {
            return Class.VascularRating > 0;
        }
    }
}
