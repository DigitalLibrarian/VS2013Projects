using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_AttackMelee : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.AttackMelee; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            game.AttackConductor.Conduct(agent, agentCommand.Target, agentCommand.AttackMove);
        }
    }
}
