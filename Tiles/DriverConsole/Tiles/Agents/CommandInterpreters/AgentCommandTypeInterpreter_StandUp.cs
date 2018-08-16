using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_StandUp : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType
        {
            get { return AgentCommandType.StandUp; } // what is the deal with airport security?
        }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            if (agent.StandUp())
            {
                game.ActionLog.AddLine(string.Format("The {0} stands up.", agent.Name));
            }
        }
    }
}
