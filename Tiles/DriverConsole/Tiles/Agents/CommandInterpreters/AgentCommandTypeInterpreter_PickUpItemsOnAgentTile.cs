using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_PickUpItemsOnAgentTile : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.PickUpItemOnAgentTile; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            // TODO - use TileOffset to grab items at relative offset from agent
            var tile = game.Atlas.GetTileAtPos(agent.Pos);
            var item = agentCommand.Item;
            if (tile.Items.Contains(item))
            {
                tile.Items.Remove(item);
                agent.Inventory.AddItem(item);
                game.ActionLog.AddLine(string.Format("The {0} picked up a {1}.", agent.Name, item.Class.Name));
            }
            else
            {
                game.ActionLog.AddLine(string.Format("The {0} reaches for the {1}, but its not there!", agent.Name, item.Class.Name));

            }
        }
    }
}
