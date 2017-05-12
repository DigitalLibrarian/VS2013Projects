using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies.Injuries;
using Tiles.Math;

namespace Tiles.Bodies
{
    public class TissueLayerWound : ITissueLayerWound
    {
        public Fraction EffectFraction { get; set; }

        // probably strain based
        public Fraction DentFraction { get; set; }

        // calculated based on penetration percentage (denom is layer thickness)
        public Fraction CutFraction { get; set; }

        // affected surface area
        public double WoundArea { get; set; }

        // ratio of layer surface area to wounded area
        public double WoundAreaRatio { get; set; }

        public double PenetrationRatio { get; set; }

        public TissueLayerWound()
        {
            const int denom = 10000;
            EffectFraction = new Fraction(0, denom);
            DentFraction = new Fraction(0, denom);
            CutFraction = new Fraction(0, denom);
        }

        public bool IsPulped()
        {
            return DentFraction.AsDouble() >= 2.5d
                || CutFraction.AsDouble() >= 1d;
        }

        public void Add(ITissueLayerInjury injry)
        {
            throw new NotImplementedException();
        }
    }
}
