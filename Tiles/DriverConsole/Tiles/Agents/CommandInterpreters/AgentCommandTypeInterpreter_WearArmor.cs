using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_WearArmor : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.WearArmor; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            agent.Outfit.Wear(agentCommand.Armor);
            agent.Inventory.AddToWorn(agentCommand.Armor, agentCommand.Armor);
            game.ActionLog.AddLine(string.Format("The {0} puts on the {1}.", agent.Name, agentCommand.Armor.Class.Name));
        }
    }
}
