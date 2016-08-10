using System;
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

        public Vector2 TileOffset { get; set; }
        public Vector2 Direction { get; set; }

        public IAgent Target { get; set; }
        public IAttackMove AttackMove { get; set; }
        public IItem Item { get; set; }
        public IWeapon Weapon { get; set; }
        public IArmor Armor { get; set; }

        public long RequiredTime { get; set; }
    }
}
