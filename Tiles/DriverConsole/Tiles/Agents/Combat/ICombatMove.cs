using System;
using System.Collections.Generic;
using Tiles.Bodies;
using Tiles.Items;
namespace Tiles.Agents.Combat
{
    public interface ICombatMove
    {
        string Name { get; set; }
        ICombatMoveClass Class { get; }

        IAgent Attacker { get; set; }
        IAgent Defender { get; set; }

        IBodyPart AttackerBodyPart { get; set; }
        IBodyPart DefenderBodyPart { get; set; }
        IItem Weapon { get; set; }
        uint PredictedDamage { get; set; }
    }
}
