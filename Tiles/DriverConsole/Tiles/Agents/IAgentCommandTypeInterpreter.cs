using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents
{
    public interface IAgentCommandTypeInterpreter
    {
        AgentCommandType CommandType { get; }
        void Execute(IGame game, IAgent agent, IAgentCommand agentCommand);
    }
}
