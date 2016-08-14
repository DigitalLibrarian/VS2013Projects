﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Combat
{
    public class CombatMoveContext : ICombatMoveContext
    {
        public ICombatMove Move { get; set; }

        public IAgent Attacker { get; set; }
        public IAgent Defender { get; set; }
    }
}