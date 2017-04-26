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
        public Fraction DentFraction { get; set; }
        public Fraction CutFraction { get; set; }
        public double AreaPercent { get; set; }

        public TissueLayerWound()
        {
            const int denom = 10000;
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
