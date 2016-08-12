using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_WieldWeapon : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.WieldWeapon; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            agent.Outfit.Wield(agentCommand.Weapon);
            agent.Inventory.AddToWorn(agentCommand.Weapon, agentCommand.Weapon);
            game.ActionLog.AddLine(string.Format("The {0} starts wielding the {1}.", agent.Name, agentCommand.Weapon.Name));
        }
    }
}
