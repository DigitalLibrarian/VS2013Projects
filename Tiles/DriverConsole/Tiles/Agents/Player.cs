using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;
using Tiles.Math;
using Tiles.Bodies;
using Tiles.Agents.Behaviors;

namespace Tiles.Agents
{
    public class Player : Agent, IPlayer
    {
        public override bool IsPlayer { get { return true; } }

        public Player(IAtlas atlas, ISprite sprite, Vector2 pos, 
            IBody body, IInventory inventory, IEquipmentSlotSet equipmentSlots,
            IAgentCommandQueueProducer commandQueue
            ) : base(atlas, sprite, pos, body, "Player", inventory, equipmentSlots)
        {
            CommandQueue = commandQueue;
        }

        public IAgent Agent
        {
            get { return this; }
        }

        IAgentCommandQueueProducer CommandQueue { get; set; }

        public void EnqueueCommand(AgentCommand command)
        {
            CommandQueue.Enqueue(command);
        }
    }
}
