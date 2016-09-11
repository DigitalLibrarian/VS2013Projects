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

        bool IsStrike { get; }

        bool IsItem { get; }

        int PrepTime { get; }
        int RecoveryTime { get; }

        StressMode StressMode { get; }

        int ContactArea { get; }
        int MaxPenetration { get; }
        int VelocityMultiplier { get; }

        IEnumerable<IBodyPartRequirement> Requirements { get; }

        bool MeetsRequirements(IBody body);

        IEnumerable<IBodyPart> GetRelatedBodyParts(IBody body);
    }

}
