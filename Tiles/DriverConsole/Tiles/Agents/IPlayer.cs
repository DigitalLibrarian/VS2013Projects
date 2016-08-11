using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tiles.Items;
using Tiles.Math;
using Tiles.Bodies;

namespace Tiles.Agents
{
    public interface IPlayer
    {
        IAtlas Atlas { get; }
        IAgent Agent { get; }
        // TODO - remove this, it is available via IAgent
        IInventory Inventory { get; }
        Vector2 Pos { get; }
        
        bool Move(Vector2 delta);


        IAgentCommand LastCommand { get; }
        void EnqueueCommand(IAgentCommand command);

        bool HasCommands { get; }
    }

}
