using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Agents.Combat
{
    public interface ICombatMoveClass
    {
        string Name { get; }
        IVerb Verb { get; }

        BodyStateChange AttackerBodyStateChange { get; }

        bool IsDefenderPartSpecific { get; }
        bool IsMartialArts { get; }
        bool IsGrabRequired { get; }
        bool CanLatch { get; set; }
        bool IsStrike { get; }
        bool IsItem { get; }
        bool IsEdged { get; set; }

        int PrepTime { get; }
        int RecoveryTime { get; }

        StressMode StressMode { get; }

        double ContactArea { get; }
        double MaxPenetration { get; }
        int VelocityMultiplier { get; }

        IEnumerable<IBodyPartRequirement> Requirements { get; }

        bool MeetsRequirements(IBody body);

        IEnumerable<IBodyPart> GetRelatedBodyParts(IBody body);
    }

}
