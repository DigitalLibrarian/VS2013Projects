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
using Tiles.Agents.Combat;
using Tiles.Materials;
using Tiles.Bodies.Injuries;
using Tiles.Bodies.Wounds;

namespace Tiles.Agents
{
    public interface IAgent
    {
        int EntityId { get; set; }

        string Name { get; }
        bool IsPlayer { get; }
        bool IsDead { get; }
        bool IsWoke { get; }
        bool IsUndead { get; }
        bool IsProne { get; }
        bool CanStand { get; }

        Vector3 Pos { get; }
        Sprite Sprite { get; }

        IAgentClass Class { get; }
        IBody Body { get; }
        IInventory Inventory { get; }
        IOutfit Outfit { get; }

        IAgentBehavior AgentBehavior { get; set; }
        IAgentCommandQueue CommandQueue { get; }

        bool CanMove(Vector3 move);
        bool Move(Vector3 move);

        bool CanPerform(ICombatMove move);
        double GetStrikeMomentum(ICombatMove move);
        IMaterial GetStrikeMaterial(ICombatMove move);

        int GetPainThreshold();
        bool CanWake();

        bool StandUp();
        bool LayDown();

        void AddInjury(IBodyPartInjury injury, IBodyPartWoundFactory woundFactory);
        void Sever(IBodyPart part);
    }
}
