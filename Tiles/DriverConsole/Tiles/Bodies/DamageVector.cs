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

        public Fraction EffectFraction { get; private set; }

        // looks like it is based off the surface percentage
        public Fraction DentFraction { get; private set; }

        public Fraction CutFraction { get; private set; }
    }
}
