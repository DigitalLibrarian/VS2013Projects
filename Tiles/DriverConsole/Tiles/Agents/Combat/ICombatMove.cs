﻿using System;
using System.Collections.Generic;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Materials;
using Tiles.Math;
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

        double Sharpness { get; }
        Vector3 Direction { get; }

        bool IsDodged { get; }
        void MarkDodged();

        bool CanPerform(IAgent agent);
        double GetStrikeMomentum();
        IMaterial GetStrikeMaterial();
    }
}
