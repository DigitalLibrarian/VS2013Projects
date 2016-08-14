using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_UnwieldWeapon : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.UnwieldWeapon; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            agent.Outfit.Unwield(agentCommand.Weapon);
            agent.Inventory.RestoreFromWorn(agentCommand.Weapon);
            game.ActionLog.AddLine(string.Format("The {0} stops wielding the {1}.", agent.Name, agentCommand.Weapon.Class.Name));
        }
    }
}
