using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_LayDown : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType
        {
            get { return AgentCommandType.LayDown; }
        }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            if (agent.LayDown())
            {
                game.ActionLog.AddLine(string.Format("The {0} lays down.", agent.Name));
            }
        }
    }
}
