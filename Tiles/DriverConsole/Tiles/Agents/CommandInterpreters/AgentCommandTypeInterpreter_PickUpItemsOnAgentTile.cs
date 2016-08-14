using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_PickUpItemsOnAgentTile : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.PickUpItemsOnAgentTile; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            // TODO - use TileOffset to grab items at relative offset from agent
            var tile = game.Atlas.GetTileAtPos(agent.Pos);
            foreach (var item in tile.Items)
            {
                agent.Inventory.AddItem(item);
                game.ActionLog.AddLine(string.Format("The {0} picked up a {1}.", agent.Name, item.Class.Name));
            }
            tile.Items.Clear();
        }
    }
}
