using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Combat.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_WearArmor : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.WearArmor; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            agent.Outfit.Wear(agentCommand.Item);
            agent.Inventory.AddToWorn(agentCommand.Armor, agentCommand.Item);
            game.ActionLog.AddLine(string.Format("The {0} puts on the {1}.", agent.Name, agentCommand.Item.Name));
        }
    }
}
