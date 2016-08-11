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
        long MinTime = 1;
        public IAgentCommand Nothing(IAgent agent)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.None,
                RequiredTime = MinTime
            };
        }

        public IAgentCommand PickUpItemsOnAgentTile(IAgent agent) 
        {
            // TODO - axe this command and replace with a single item pick up command (easier to handle time req.)
            return new AgentCommand
            {
                CommandType = AgentCommandType.PickUpItemsOnAgentTile,
                RequiredTime = MinTime * 30
            };
        }

        public IAgentCommand MeleeAttack(IAgent agent, IAgent target, IAttackMove attackMove)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.AttackMelee,
                RequiredTime = MinTime * 20,
                Target = target,
                AttackMove = attackMove
            };
        }

        public IAgentCommand WieldWeapon(IAgent agent, IItem item, IWeapon weapon)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.WieldWeapon,
                RequiredTime = MinTime * 5,
                Item = item,
                Weapon = weapon
            };
        }

        public IAgentCommand WearArmor(IAgent agent, IItem item, IArmor armor)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.WearArmor,
                RequiredTime = MinTime * 5,
                Item = item,
                Armor = armor
            };
        }

        public IAgentCommand MoveDirection(IAgent agent, Vector2 direction)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.Move,
                RequiredTime = MinTime * 10,
                Direction = direction
            };
        }

        public IAgentCommand UnwieldWeapon(IAgent agent, IItem item, IWeapon weapon)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.UnwieldWeapon,
                RequiredTime = MinTime * 5,
                Item = item,
                Weapon = weapon
            };
        }

        public IAgentCommand TakeOffArmor(IAgent agent, IItem item, IArmor armor)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.TakeOffArmor,
                RequiredTime = MinTime * 5,
                Item = item,
                Armor = armor
            };
        }

        public IAgentCommand DropInventoryItem(IAgent agent, IItem item)
        {
            return new AgentCommand
            {
                CommandType = AgentCommandType.DropInventoryItem,
                RequiredTime = MinTime,
                Item = item
            };
        }
    }
}
