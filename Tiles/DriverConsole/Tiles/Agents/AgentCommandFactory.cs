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
        public IEnumerable<IAgentCommand> Nothing(IAgent agent)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.None,
                RequiredTime = MinTime * 1
            };
        }
        
        public IEnumerable<IAgentCommand> MoveDirection(IAgent agent, Vector3 direction)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.Move,
                RequiredTime = MinTime * 10,
                Direction = direction
            };
        }

        public IEnumerable<IAgentCommand> PickUpItemsOnAgentTile(IGame game, IAgent agent) 
        {
            foreach (var item in game.Atlas.GetTileAtPos(agent.Pos).Items)
            {
                yield return new AgentCommand
                {
                    CommandType = AgentCommandType.PickUpItemOnAgentTile,
                    RequiredTime = MinTime*2,
                    Item = item
                };
            }
        }

        public IEnumerable<IAgentCommand> MeleeAttack(IAgent agent, IAgent target, ICombatMove attackMove)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.None,
                RequiredTime = attackMove.Class.PrepTime
            };
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.AttackMelee,
                RequiredTime = MinTime,
                Target = target,
                AttackMove = attackMove
            };
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.None,
                RequiredTime = attackMove.Class.RecoveryTime
            };
        }

        public IEnumerable<IAgentCommand> WieldWeapon(IAgent agent, IItem item)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.WieldWeapon,
                RequiredTime = MinTime * 5,
                Weapon = item
            };
        }

        public IEnumerable<IAgentCommand> WearArmor(IAgent agent, IItem item)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.WearArmor,
                RequiredTime = MinTime * 5,
                Armor = item
            };
        }

        public IEnumerable<IAgentCommand> UnwieldWeapon(IAgent agent, IItem item)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.UnwieldWeapon,
                RequiredTime = MinTime * 5,
                Weapon = item,
            };
        }

        public IEnumerable<IAgentCommand> TakeOffArmor(IAgent agent, IItem item)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.TakeOffArmor,
                RequiredTime = MinTime * 5,
                Armor = item
            };
        }

        public IEnumerable<IAgentCommand> DropInventoryItem(IAgent agent, IItem item)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.DropInventoryItem,
                RequiredTime = MinTime,
                Item = item
            };
        }
    }
}
