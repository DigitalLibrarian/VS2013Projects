using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents
{
    public class AgentCommandInterpreter : IAgentCommandInterpreter
    {
        IDictionary<AgentCommandType, IAgentCommandTypeInterpreter> TypeInterpreters { get; set; }

        public AgentCommandInterpreter(IDictionary<AgentCommandType, IAgentCommandTypeInterpreter> typeInterpreters)
        {
            TypeInterpreters = typeInterpreters;
        }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            if (TypeInterpreters.ContainsKey(agentCommand.CommandType))
            {
                TypeInterpreters[agentCommand.CommandType].Execute(game, agent, agentCommand);
            }
            else
            {
                throw new NotImplementedException(string.Format("AgentCommandType {0} has no interpreter", agentCommand.CommandType));
            }
        }
    }
}
