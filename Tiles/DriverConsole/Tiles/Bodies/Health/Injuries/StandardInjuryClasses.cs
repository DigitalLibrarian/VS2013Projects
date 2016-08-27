using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Health.Injuries
{
    public static class StandardInjuryClasses
    {
        private const int BruisedTtl = 50;
        private const int TornTtl = 80;
        private const int PuncturedTtl = 200;
        public static readonly IInjuryClass BruisedTissueLayer = new InjuryClass()
        {
            Adjective = "bruised",
            IsBodyPartSpecific = true,
            IsTissueLayerSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = BruisedTtl
        };

        public static readonly IInjuryClass TornTissueLayer = new InjuryClass()
        {
            Adjective = "torn",
            IsBodyPartSpecific = true,
            IsTissueLayerSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = TornTtl
        };

        public static readonly IInjuryClass PuncturedTissueLayer = new InjuryClass()
        {
            Adjective = "punctured",
            IsBodyPartSpecific = true,
            IsTissueLayerSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = PuncturedTtl
        };

        public static readonly IInjuryClass MissingBodyPart = new InjuryClass()
        {
            Adjective = "missing",
            IsBodyPartSpecific = true,
            IsPermanant = true,
            CripplesBodyPart = true
        };
    }
}
