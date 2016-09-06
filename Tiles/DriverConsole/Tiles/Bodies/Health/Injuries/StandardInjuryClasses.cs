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

        #region body part injuries

        public static readonly IInjuryClass MissingBodyPart = new InjuryClass()
        {
            Adjective = "missing",
            IsBodyPartSpecific = true,
            IsPermanant = true,
            CripplesBodyPart = true,
            RemovesBodyPart = true,
            Severity = InjurySeverity.LoppedOff
        };

        public static readonly IInjuryClass BrokenBodyPart = new InjuryClass()
        {
            Adjective = "broken",
            IsBodyPartSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = TornTtl,
            Severity = InjurySeverity.Broken
        };

        public static readonly IInjuryClass BruisedBodyPart = new InjuryClass()
        {
            Adjective = "bruised",
            IsBodyPartSpecific = true,
            IsTissueLayerSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = BruisedTtl,
            Severity = InjurySeverity.LightlyWounded
        };

        public static readonly IInjuryClass BatteredBodyPart = new InjuryClass()
        {
            Adjective = "battered",
            IsBodyPartSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = TornTtl,
            Severity = InjurySeverity.ModeratelyWounded
        };
        
        public static readonly IInjuryClass MangledBodyPart = new InjuryClass()
        {
            Adjective = "mangled",
            IsBodyPartSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = TornTtl,
            Severity = InjurySeverity.Mangled
        };

        public static readonly IInjuryClass CutBodyPart = new InjuryClass()
        {
            Adjective = "cut",
            IsBodyPartSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = TornTtl,
            Severity = InjurySeverity.LightlyWounded
        };


        public static readonly IInjuryClass BadlyGashedBodyPart = new InjuryClass()
        {
            Adjective = "badly gashed",
            IsBodyPartSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = TornTtl,
            Severity = InjurySeverity.ModeratelyWounded
        };


        public static readonly IInjuryClass PiercedBodyPart = new InjuryClass()
        {
            Adjective = "pierced",
            IsBodyPartSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = TornTtl,
            Severity = InjurySeverity.LightlyWounded
        };


        public static readonly IInjuryClass BadlyPiercedBodyPart = new InjuryClass()
        {
            Adjective = "badly pierced",
            IsBodyPartSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = TornTtl,
            Severity = InjurySeverity.ModeratelyWounded
        };

        public static readonly IInjuryClass TornBodyPart = new InjuryClass()
        {
            Adjective = "torn",
            IsBodyPartSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = TornTtl,
            Severity = InjurySeverity.LightlyWounded
        };


        public static readonly IInjuryClass BadlyRippedBodyPart = new InjuryClass()
        {
            Adjective = "badly ripped",
            IsBodyPartSpecific = true,
            CanBeHealed = true,
            UsesTtl = true,
            Ttl = TornTtl,
            Severity = InjurySeverity.ModeratelyWounded
        };

        #endregion
    }
}
