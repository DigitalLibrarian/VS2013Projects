using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Items;
using Tiles.Math;
using Tiles.Bodies;
using Tiles.Items.Outfits;
using Tiles.Agents.Behaviors;

namespace Tiles.Agents
{
    public interface IAgent
    {
        int EntityId { get; set; }

        string Name { get; }
        IAgentBehavior AgentBehavior { get; set; }
        ISprite Sprite { get; }
        Vector3 Pos { get; }
        IBody Body { get; }
        bool IsPlayer { get; }
        bool IsDead { get; }
        bool IsUndead { get; }
        bool CanMove(Vector3 move);
        bool Move(Vector3 move);
        void Update(IGame game);
        IInventory Inventory { get; }
        IOutfit Outfit { get; }

        IAgentCommandQueue CommandQueue { get; }
    }
}
