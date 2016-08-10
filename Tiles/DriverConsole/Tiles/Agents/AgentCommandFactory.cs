using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Agents
{
    public class AgentCommandFactory : IAgentCommandFactory
    {
        public IAgentCommand Nothing(IAgent agent)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.None
            };
        }

        public IAgentCommand PickUpItemsOnAgentTile(IAgent agent) 
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.PickUpItemsOnAgentTile
            };
        }

        public IAgentCommand MeleeAttack(IAgent agent, IAgent target, IAttackMove attackMove)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.AttackMelee,
                Target = target,
                AttackMove = attackMove
            };
        }

        public IAgentCommand WieldWeapon(IAgent agent, IItem item, IWeapon weapon)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.WieldWeapon,
                Item = item,
                Weapon = weapon
            };
        }

        public IAgentCommand WearArmor(IAgent agent, IItem item, IArmor armor)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.WearArmor,
                Item = item,
                Armor = armor
            };
        }

        public IAgentCommand MoveDirection(IAgent agent, Vector2 direction)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.Move,
                Direction = direction
            };
        }

        public IAgentCommand UnwieldWeapon(IAgent agent, IItem item, IWeapon weapon)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.UnwieldWeapon,
                Item = item,
                Weapon = weapon
            };
        }

        public IAgentCommand TakeOffArmor(IAgent agent, IItem item, IArmor armor)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.TakeOffArmor,
                Item = item,
                Armor = armor
            };
        }

        public IAgentCommand DropInventoryItem(IAgent agent, IItem item)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.DropInventoryItem,
                Item = item
            };
        }
    }
}
