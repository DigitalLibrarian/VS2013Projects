using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Bodies
{
    public interface IDamageVector
    {
        Fraction EffectFraction { get; }
        Fraction DentFraction { get; }
        Fraction CutFraction { get; }

        bool IsPulped();
    }

    public class DamageVector : IDamageVector
    {
        public DamageVector()
        {
            const int denom = 10000;
            EffectFraction = new Fraction(0, denom);
            DentFraction = new Fraction(0, denom);
            CutFraction = new Fraction(0, denom);
        }

        public virtual Fraction EffectFraction { get; private set; }

        // looks like it is based off the surface percentage
        public virtual Fraction DentFraction { get; private set; }

        public virtual Fraction CutFraction { get; private set; }

        public virtual bool IsPulped() { 
            return EffectFraction.AsDouble() >= 1d
                || DentFraction.AsDouble() >= 2.5d
                || CutFraction.AsDouble() >= 1d;
        }
    }
}
