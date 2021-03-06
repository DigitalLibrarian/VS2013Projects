﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies.Injuries;
using Tiles.Bodies.Wounds;
using Tiles.Math;

namespace Tiles.Bodies
{
    public interface IBody
    {
        bool IsGrasping { get; }
        bool IsBeingGrasped { get; }
        bool IsWrestling { get; }
        bool IsDead { get; }

        int TotalPain { get; }
        int TotalBleeding { get; }

        double Size { get; }

        Fraction Blood { get; set; }

        IList<IBodyPart> Parts { get; }
        IBodyClass Class { get; }
        IList<IBodyPartWound> Wounds { get; }

        void Amputate(IBodyPart part);

        IEnumerable<ICombatMoveClass> Moves { get; set; }
        IEnumerable<IBodyPart> GetInternalParts(IBodyPart part);

        void SetAttribute(string name, int value);
        int GetAttribute(string name);

        IDictionary<IBodyPart, int> GetRelations(IBodyPart target, BodyPartRelationType type);

        void AddInjury(IBodyPartInjury injury, IBodyPartWoundFactory woundFactory);
    }
}
