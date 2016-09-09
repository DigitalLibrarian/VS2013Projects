using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Injuries
{
    public interface ITissueLayerInjuryClass
    {
        string Adjective { get; }
        string Gerund { get; }

        DamageType DamageType { get; set; }

        bool IsInRange(double dVal);
    }

    public class TissueLayerInjuryClass : ITissueLayerInjuryClass
    {
        public string Adjective { get; set; }
        public string Gerund { get; set; }
        public DamageType DamageType { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public bool IsUpperBound { get; set; }
        public bool IsLowerBound { get; set; }

        public bool IsInRange(double dVal)
        {
            if (dVal <= 0) return false;
            if (IsUpperBound)
            {
                return Min <= dVal;
            }

            if (IsLowerBound)
            {
                return dVal <= Max;
            }
            return Min < dVal && dVal <= Max;
        }
    }
}
