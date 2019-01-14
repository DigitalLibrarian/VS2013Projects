using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_Dodge : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.Dodge; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            var originalPos = agent.Pos;
            if (agent.Move(agentCommand.Direction))
            {
                // this is the enemies move, in the case of a dodge command
                agentCommand.AttackMove.MarkDodged();
            }
        }
    }
}
