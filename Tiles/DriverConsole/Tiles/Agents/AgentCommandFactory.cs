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

        private long ScaleTime(IAgent agent, long amount=1)
        {
            return amount * (agent.IsProne ? 3 : 1);
        }

        public IEnumerable<IAgentCommand> Nothing(IAgent agent)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.None,
                RequiredTime = ScaleTime(agent) * 1
            };
        }
        
        public IEnumerable<IAgentCommand> MoveDirection(IAgent agent, Vector3 direction)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.Move,
                RequiredTime = ScaleTime(agent, 10),
                Direction = direction
            };
        }

        public IEnumerable<IAgentCommand> DodgeDirection(IAgent agent, IAgent other, Vector3 direction)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.Dodge,
                RequiredTime = ScaleTime(agent, 1),
                Direction = direction, 
                AttackMove = other.AgentBehavior.Context.Command.AttackMove
            };

            var nextCommand = other.CommandQueue.Peek();
            if (nextCommand != null)
            {
                yield return new AgentCommand
                {
                    CommandType = AgentCommandType.None,
                    RequiredTime = other.AgentBehavior.Context.TimeRemaining + nextCommand.RequiredTime
                };
            }
        }

        public IEnumerable<IAgentCommand> PickUpItemsOnAgentTile(IGame game, IAgent agent) 
        {
            foreach (var item in game.Atlas.GetTileAtPos(agent.Pos).Items)
            {
                yield return new AgentCommand
                {
                    CommandType = AgentCommandType.PickUpItemOnAgentTile,
                    RequiredTime = ScaleTime(agent, 2),
                    Item = item
                };
            }
        }

        public IEnumerable<IAgentCommand> MeleeAttack(IAgent agent, IAgent target, ICombatMove attackMove)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.AttackMeleePrep,
                RequiredTime = ScaleTime(agent, attackMove.Class.PrepTime),
                Target = target,
                AttackMove = attackMove
            };
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.AttackMelee,
                RequiredTime = ScaleTime(agent),
                Target = target,
                AttackMove = attackMove
            };
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.AttackMeleeRecovery,
                RequiredTime = ScaleTime(agent, attackMove.Class.RecoveryTime),
                Target = target,
                AttackMove = attackMove
            };
        }

        public IEnumerable<IAgentCommand> WieldWeapon(IAgent agent, IItem item)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.WieldWeapon,
                RequiredTime = ScaleTime(agent, 5),
                Weapon = item
            };
        }

        public IEnumerable<IAgentCommand> WearArmor(IAgent agent, IItem item)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.WearArmor,
                RequiredTime = ScaleTime(agent, 5),
                Armor = item
            };
        }

        public IEnumerable<IAgentCommand> UnwieldWeapon(IAgent agent, IItem item)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.UnwieldWeapon,
                RequiredTime = ScaleTime(agent, 5),
                Weapon = item,
            };
        }

        public IEnumerable<IAgentCommand> TakeOffArmor(IAgent agent, IItem item)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.TakeOffArmor,
                RequiredTime = ScaleTime(agent, 5),
                Armor = item
            };
        }

        public IEnumerable<IAgentCommand> DropInventoryItem(IAgent agent, IItem item)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.DropInventoryItem,
                RequiredTime = ScaleTime(agent),
                Item = item
            };
        }

        public IEnumerable<IAgentCommand> StandUp(IAgent agent)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.StandUp,
                RequiredTime = ScaleTime(agent)
            };
        }

        public IEnumerable<IAgentCommand> LayDown(IAgent agent)
        {
            yield return new AgentCommand
            {
                CommandType = AgentCommandType.LayDown,
                RequiredTime = ScaleTime(agent)
            };
        }
    }
}
