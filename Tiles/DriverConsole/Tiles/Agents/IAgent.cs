using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Items;
using Tiles.Math;
using Tiles.Bodies;
using Tiles.Items.Outfits;

namespace Tiles.Agents
{
    public interface IAgent
    {
        string Name { get; }
        IAgentBehavior AgentBehavior { get; set; }
        ISprite Sprite { get; }
        Vector2 Pos { get; }
        IBody Body { get; }
        bool IsPlayer { get; }
        bool IsDead { get; }
        bool IsUndead { get; }
        bool CanMove(Vector2 move);
        bool Move(Vector2 move);
        void Update(IGame game);
        IInventory Inventory { get; }
        IOutfit Outfit { get; }
    }
}
