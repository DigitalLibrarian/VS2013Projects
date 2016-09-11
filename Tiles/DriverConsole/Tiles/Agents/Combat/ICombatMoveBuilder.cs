using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;

namespace Tiles.Agents.Combat
{
    // TODO - rename as *Factory.  this is not a builder
    public interface ICombatMoveBuilder
    {
        ICombatMove AttackBodyPartWithWeapon(IAgent attacker, IAgent defender, ICombatMoveClass moveClass, IBodyPart targetBodyPart, IItem weapon);

        ICombatMove GraspOpponentBodyPart(IAgent attacker, IAgent defender, IBodyPart attackerPart, IBodyPart defenderBodyPart);

        ICombatMove PullGraspedBodyPart(IAgent attacker, IAgent defender, IBodyPart mePart, IBodyPart youPart);
        ICombatMove ReleaseGraspedPart(IAgent attacker, IAgent defender, IBodyPart mePart, IBodyPart youPart);
        ICombatMove BreakOpponentGrasp(IAgent attacker, IAgent defender, IBodyPart attackerBodyPart, IBodyPart defenderBodyPart);

        ICombatMove BodyMove(IAgent attacker, IAgent defender, ICombatMoveClass moveClass, IBodyPart youPart);
    }
}
