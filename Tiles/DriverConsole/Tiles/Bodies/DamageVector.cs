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

        bool IsPristine();
        bool IsPulped();

        void Add(IDamageVector damage);
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

        public virtual bool IsPulped() 
        { 
            return EffectFraction.AsDouble() >= 1d
                || DentFraction.AsDouble() >= 1d
                || CutFraction.AsDouble() >= 1d;
        }

        public virtual bool IsPristine()
        {
            return EffectFraction.Numerator == 0
                && DentFraction.Numerator == 0
                && CutFraction.Numerator == 0;
        }

        public void Add(IDamageVector damage)
        {
            EffectFraction.Numerator += damage.EffectFraction.Numerator;
            CutFraction.Numerator += damage.CutFraction.Numerator;
            DentFraction.Numerator += damage.DentFraction.Numerator;
        }
    }
}
