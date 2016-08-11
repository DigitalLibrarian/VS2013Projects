using System;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Math;
namespace Tiles.Agents
{
    public interface IAgentCommand
    {
        AgentCommandType CommandType { get; set; }
        Vector2 Direction { get; set; }
        Vector2 TileOffset { get; set; }

        IAttackMove AttackMove { get; set; }
        IAgent Target { get; set; }

        IItem Item { get; set; }
        IItem Weapon { get; set; }
        IItem Armor { get; set; }

        // TODO - test this in agent command factory
        long RequiredTime { get; set; }
    }
}
