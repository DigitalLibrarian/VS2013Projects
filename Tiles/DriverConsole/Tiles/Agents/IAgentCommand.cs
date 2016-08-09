using System;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Math;
namespace Tiles.Agents
{
    public interface IAgentCommand
    {
        IArmor Armor { get; set; }
        IAttackMove AttackMove { get; set; }
        AgentCommandType CommandType { get; set; }
        Vector2 Direction { get; set; }
        IItem Item { get; set; }
        IAgent Target { get; set; }
        Vector2 TileOffset { get; set; }
        IWeapon Weapon { get; set; }
    }
}
