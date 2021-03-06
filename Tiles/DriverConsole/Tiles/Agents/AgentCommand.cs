﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Agents
{
    public class AgentCommand : IAgentCommand
    {
        public AgentCommandType CommandType { get; set; }

        public Vector3 TileOffset { get; set; }
        public Vector3 Direction { get; set; }

        public IAgent Target { get; set; }
        public ICombatMove AttackMove { get; set; }
        public IItem Item { get; set; }
        public IItem Weapon { get; set; }
        public IItem Armor { get; set; }

        public long RequiredTime { get; set; }
    }
}
