using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Items.Outfits;
using Tiles.Agents.Behaviors;
using Tiles.Agents.Combat;
namespace Tiles.Agents
{
    public class Agent : IAgent
    {
        public int EntityId { get; set; }
        public IAtlas Atlas { get; private set; }
        public IAgentClass Class { get; private set; }
        public string Name { get { return Class.Name; } }
        public ISprite Sprite { get { return Class.Sprite; } }
        public IBody Body { get; private set; }
        public Vector3 Pos { get; protected set; }
        public IInventory Inventory { get; private set; }
        public virtual bool IsPlayer { get { return false; } }
        public IAgentBehavior AgentBehavior { get; set; }
        public IOutfit Outfit { get; private set; }
        public bool IsUndead { get; set; }

        public IAgentCommandQueue CommandQueue { get; private set; }

        public bool IsDead { 
            get 
            {
                return !Body.Parts.Any()
                    || Body.Parts.First().IsDestroyed();
            } 
        }

        public Agent(
            IAtlas atlas, 
            IAgentClass agentClass,
            Vector3 pos, 
            IBody body, 
            IInventory inventory, 
            IOutfit outfit,
            IAgentCommandQueue commandQueue)
        {
            Atlas = atlas;
            Class = agentClass;
            Pos = pos;
            Body = body;
            Inventory = inventory;
            Outfit = outfit;
            
            CommandQueue = commandQueue;
        }

        public virtual void Update(IGame game)
        {
            if (AgentBehavior != null)
            {
                AgentBehavior.Update(game, this);
            }
        }

        public bool CanMove(Vector3 delta)
        {
            if (Body.IsWrestling) return false;

            var newTile = Atlas.GetTileAtPos(Pos + delta);
            if (newTile == null) return false;
            if (newTile.HasAgent) return false;

            if (newTile.HasStructureCell)
            {
                return newTile.StructureCell.CanPass;
            }

            return newTile.IsTerrainPassable;
        }

        public bool Move(Vector3 move)
        {
            if (CanMove(move))
            {
                var startTile = Atlas.GetTileAtPos(Pos);
                startTile.RemoveAgent();
                Pos += move;
                var newTile = Atlas.GetTileAtPos(Pos);
                newTile.SetAgent(this);
                return true;
            }
            return false;
        }

        public double GetStrikeMomentum(ICombatMove move)
        {
            double Str, VelocityMultiplier, Size, Fat, W;
            Str = 1250;
            Size = Body.Size;
            Fat = 1d;
            // M = (Str * VelocityMultiplier) / ((10^6/Size) + ((10 * F) / W))
            if (move.Class.IsItem)
            {
                var weapon = move.Weapon;
                VelocityMultiplier = (double)move.Class.VelocityMultiplier / 1000d;
                W = weapon.GetMass() / 1000d;
            }
            else
            {
                double mass = move.Class.GetRelatedBodyParts(move.Attacker.Body)
                    .Select(p => p.GetMass())
                    .Sum();

                VelocityMultiplier = 1.0;
                W = mass / 1000d;
            }
            return (Str * VelocityMultiplier)
                / ((1000000d / Size) + ((10 * Fat) / W));
        }
    }
}
