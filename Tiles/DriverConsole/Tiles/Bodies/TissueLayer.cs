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

        public string Name { get { return Material.Name; } }

        public TissueLayer(ITissueLayerClass layerClass, IMaterial material, double thickness, double volume)
        {
            Class = layerClass;
            Material = material;
            Thickness = thickness;
            Volume = volume;

            const int denom = 10000;
            EffectFraction = new Fraction(0, denom);
            DentFraction = new Fraction(0, denom);
            CutFraction = new Fraction(0, denom);
        }

        public Fraction EffectFraction { get; private set; }

        // looks like it is based off the surface percentage
        public Fraction DentFraction { get; private set; }

        public Fraction CutFraction { get; private set; }

        // affected surface area
        public double WoundArea { get; set; }

        // ratio of layer surface area to wounded area
        public double WoundAreaRatio { get; set; }

        // ratio of the penetrated depth
        public double PenetrationRatio { get; set; }

        private IEnumerable<Fraction> GetDamageFractions() 
        { 
            yield return EffectFraction;
            yield return DentFraction;
            yield return CutFraction;
        }

        public bool IsPulped()
        {
            return GetDamageFractions().Any(x => x.AsDouble() >= 1d);
        }

        public bool IsVascular()
        {
            return Class.VascularRating > 0;
        }

        public bool IsSoft(StressMode stressMode)
        {
            if (stressMode == StressMode.Edge)
            {
                return Material.ImpactStrainAtYield >= 50000;
            }
            else
            {
                return Material.ShearStrainAtYield >= 50000;
            }
        }
    }
}
