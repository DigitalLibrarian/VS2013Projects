using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_None : IAgentCommandTypeInterpreter
    {
        public virtual AgentCommandType CommandType { get { return AgentCommandType.None; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand) { }
    }
}
