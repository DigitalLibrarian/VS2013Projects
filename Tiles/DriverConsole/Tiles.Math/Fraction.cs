using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Math
{
    public class Fraction
    {
        public long Numerator { get; set; }
        private long _denom;
        public long Denominator { 
            get { return _denom; }
            set {
                CheckDenomZero(value);
                _denom = value;
            }
        }

        public Fraction(int num, int denom) : this((long)num, (long)denom) { }

        public Fraction(long num, long denom)
        {
            Numerator = num;
            Denominator = denom;
        }

        void CheckDenomZero(long d)
        {
            if (d == 0)
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
