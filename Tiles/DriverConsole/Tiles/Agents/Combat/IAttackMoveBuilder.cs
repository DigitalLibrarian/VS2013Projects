﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;

namespace Tiles.Agents.Combat
{
    public interface IAttackMoveBuilder
    {
        IAttackMove AttackBodyPartWithWeapon(IAgent attacker, IAgent defender, IAttackMoveClass moveClass, IBodyPart targetBodyPart, IWeapon weapon);
    }
}
