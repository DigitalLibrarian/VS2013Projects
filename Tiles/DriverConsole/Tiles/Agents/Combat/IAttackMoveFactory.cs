﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Random;

namespace Tiles.Agents.Combat
{
    public interface IAttackMoveFactory
    {
        IEnumerable<IAttackMove> GetPossibleMoves(IAgent attacker, IAgent defender);
    }
}
