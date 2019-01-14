using System;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Math;
namespace Tiles.Agents
{
    public interface IAgentCommand
    {
        AgentCommandType CommandType { get; }
        Vector3 Direction { get; }
        Vector3 TileOffset { get; }

        ICombatMove AttackMove { get; }
        IAgent Target { get; }

        IItem Item { get; }
        IItem Weapon { get; }
        IItem Armor { get; }

        long RequiredTime { get; }
    }
}
