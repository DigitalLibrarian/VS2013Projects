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

        public Player(IAtlas atlas, ISprite sprite, Vector3 pos, 
            IBody body, IInventory inventory,
            IOutfit outfit,
            IAgentCommandQueue commandQueue
            ) : base(atlas, sprite, pos, body, "Player", inventory, outfit)
        {
            CommandQueue = commandQueue;
        }

        public IAgent Agent
        {
            get { return this; }
        }

        IAgentCommandQueue CommandQueue { get; set; }
        public IAgentCommand LastCommand { get; private set; }
        public void EnqueueCommand(IAgentCommand command)
        {
            CommandQueue.Enqueue(command);
            LastCommand = command;
        }

        public bool HasCommands { get { return CommandQueue.Any(); } }
    }
}
