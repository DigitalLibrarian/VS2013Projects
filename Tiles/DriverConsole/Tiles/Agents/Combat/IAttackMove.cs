using System;
using System.Collections.Generic;
using Tiles.Bodies;
using Tiles.Items;
namespace Tiles.Agents.Combat
{
    public interface IAttackMove
    {
        string Name { get; set; }
        IAttackMoveClass AttackMoveClass { get; }

        IAgent Attacker { get; set; }
        IAgent Defender { get; set; }

        IBodyPart AttackerBodyPart { get; set; }
        IBodyPart DefenderBodyPart { get; set; }
        IItem Weapon { get; set; }

        uint PredictedDamage { get; set; }
    }
}
