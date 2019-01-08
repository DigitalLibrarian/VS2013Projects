using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_Move : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.Move; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            var originalPos = agent.Pos;
            if(agent.Move(agentCommand.Direction))
            {
                game.Splatter.Track(originalPos, agentCommand.Direction);
            }
        }
    }
}
