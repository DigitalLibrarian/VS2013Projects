using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Math
{
    public struct Fraction
    {
        public long Numerator { get; private set; }
        public long Denominator { get; private set; }

        public Fraction(long num, long denom)
            : this()
        {
            Numerator = num;
            Denominator = denom;

            if (Denominator == 0)
            {
                throw new DivideByZeroException();
            }
        }

        public double AsDouble()
        {
            return (double)Numerator / (double)Denominator;
        }
    }
}
