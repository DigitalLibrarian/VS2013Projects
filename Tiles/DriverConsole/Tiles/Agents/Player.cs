using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;
using Tiles.Math;
using Tiles.Bodies;
using Tiles.Agents.Behaviors;
using Tiles.Items.Outfits;

namespace Tiles.Agents
{
    public class Player : Agent, IPlayer
    {
        public override bool IsPlayer { get { return true; } }

        public Player(
            IAtlas atlas, 
            IAgentClass agentClass, 
            Vector3 pos,
            IBody body, 
            IInventory inventory,
            IOutfit outfit,
            IAgentCommandQueue commandQueue
            ) : base(atlas, agentClass, pos, body, inventory, outfit, commandQueue)
        {

        }

        public IAgent Agent
        {
            get { return this; }
        }

        public void EnqueueCommands(IEnumerable<IAgentCommand> commands)
        {
            foreach(var command in commands)
            {
                CommandQueue.Enqueue(command);
            }
        }

        public bool HasCommands { get { return CommandQueue.Any(); } }
    }
}
