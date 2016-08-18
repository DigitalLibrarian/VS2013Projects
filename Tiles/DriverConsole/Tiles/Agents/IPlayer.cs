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
        Vector3 Pos { get; }
        
        bool Move(Vector3 delta);

        void EnqueueCommands(IEnumerable<IAgentCommand> commands);
        bool HasCommands { get; }
    }

}
