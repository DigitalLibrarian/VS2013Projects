using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Bodies.Health;
using Tiles.Items;

namespace Tiles.Agents.Combat
{
    public interface IDamageCalc
    {
        uint MeleeStrikeMoveDamage(ICombatMoveClass moveClass, IAgent attacker, IAgent defender, IBodyPart bodyPart, IItem weapon);
        uint MeleeStrikeBodyPartAttackDamage(ICombatMoveClass moveClass, IAgent attacker, IAgent defender, IBodyPart attackerPart, IBodyPart defenderPart);
        uint ThrownItemDamage(IAgent agent, IAgent defender, IBodyPart bodyPart, IItem item);
    }


}
