using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_DropInventoryItem : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.DropInventoryItem; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            agent.Inventory.RemoveItem(agentCommand.Item);
            game.Atlas.GetTileAtPos(agent.Pos).Items.Add(agentCommand.Item);
            game.ActionLog.AddLine(string.Format("The {0} dropped {1}.", agent.Name, agentCommand.Item.Name));
        }
    }
}
