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
        public double WoundAreaRatio { get; private set; }
        public double PenetrationRatio { get; private set; }

        public TissueLayer(ITissueLayerClass layerClass, double thickness, double volume)
        {
            Class = layerClass;
            Thickness = thickness;
            Volume = volume;

            Damage = new DamageVector();
        }

        public bool IsPulped
        {
            get
            {
                return Damage.IsPulped && (PenetrationRatio >= 1d && WoundAreaRatio >= 1d);
            }
        }

        public bool IsPristine
        {
            get
            {
                return Damage.IsPristine && (PenetrationRatio <= 0d && WoundAreaRatio <= 0d);
            }
        }

        public bool IsVascular
        {
            get
            {
                return Class.VascularRating > 0;
            }   
        }

        public void AddInjury(ITissueLayerInjury injury)
        {
            if (injury.StressResult != StressResult.None)
            {
                // These might be better as a live sum
                WoundAreaRatio += injury.ContactAreaRatio;
                WoundAreaRatio = System.Math.Min(WoundAreaRatio, 1d);
                WoundAreaRatio = System.Math.Max(WoundAreaRatio, 0d);

                PenetrationRatio += injury.PenetrationRatio;
                PenetrationRatio = System.Math.Min(PenetrationRatio, 1d);
                PenetrationRatio = System.Math.Max(PenetrationRatio, 0d);

                Damage.Add(injury.Damage);
            }
        }
    }
}
