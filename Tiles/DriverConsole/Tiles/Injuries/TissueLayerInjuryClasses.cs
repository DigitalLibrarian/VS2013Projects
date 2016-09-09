using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Injuries
{
    public static class TissueLayerInjuryClasses
    {
        public static ITissueLayerInjuryClass Bruise = new TissueLayerInjuryClass
        {
            Adjective = "bruised",
            Gerund = "bruising",
            DamageType = DamageType.Bludgeon,
            Max = 0.3,
            IsLowerBound = true
        };

        public static ITissueLayerInjuryClass Fracture = new TissueLayerInjuryClass
        {
            Adjective = "fractured",
            Gerund = "fracturing",
            DamageType = DamageType.Bludgeon,
            Min = 0.3,
            IsUpperBound = true
        };

        public static ITissueLayerInjuryClass Tear = new TissueLayerInjuryClass
        {
            Adjective = "torn",
            Gerund = "tearing",
            DamageType = DamageType.Slash,
            Max = .3,
            IsLowerBound = true
        };

        public static ITissueLayerInjuryClass TearApart = new TissueLayerInjuryClass
        {
            Adjective = "torn apart",
            Gerund = "tearing apart",
            DamageType = DamageType.Slash,
            Min = .3,
            IsUpperBound = true,
        };
    }
}
