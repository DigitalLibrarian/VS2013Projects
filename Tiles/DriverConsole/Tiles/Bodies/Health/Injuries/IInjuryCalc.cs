using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Bodies.Health;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Bodies.Health.Injuries
{
    public interface IInjuryCalc
    {
        IEnumerable<IInjury> MeleeWeaponStrike(
            ICombatMoveClass moveClass,
            double momentum,
            IAgent attacker, IAgent defender,
            IBodyPart targetPart, IItem weapon
            );

        IEnumerable<IInjury> UnarmedStrike(
            ICombatMoveClass moveClass,
            double momentum,
            IAgent attacker, IAgent defender,
            IBodyPart targetPart
            );

    }
}
