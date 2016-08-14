using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_TakeOffArmor : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.TakeOffArmor; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            agent.Outfit.TakeOff(agentCommand.Armor);
            agent.Inventory.RestoreFromWorn(agentCommand.Armor);
            game.ActionLog.AddLine(string.Format("The {0} takes off the {1}.", agent.Name, agentCommand.Armor.Class.Name));
        }
    }
}
