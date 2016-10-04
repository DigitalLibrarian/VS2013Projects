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
using Tiles.Materials;
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
                    || Body.Parts.First().IsPulped();
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
        
        public IMaterial GetStrikeMaterial(ICombatMove move)
        {
            if (move.Class.IsItem)
            {
                return move.Weapon.Class.Material;
            }
            else
            {
                var relatedParts = move.Class.GetRelatedBodyParts(Body);
                var strikePart = relatedParts.First();
                var weaponMat = strikePart.Tissue.TissueLayers.Last().Material;
                return weaponMat;
            }
        }

        public double GetStrikeMomentum(ICombatMove move)
        {
            double baseSize = Body.Size;

            double Str, VelocityMultiplier, Size, W;
            Str = 1250;
            Size = Body.Size;

            if (move.Class.IsItem)
            {
                var weapon = move.Weapon;
                VelocityMultiplier = (double)move.Class.VelocityMultiplier;
                W = weapon.GetMass();
            }
            else
            {
                double mass = move.Class.GetRelatedBodyParts(move.Attacker.Body)
                    .Select(p => p.GetMass())
                    .Sum();

                VelocityMultiplier = 1000d;
                W = mass;
            }

            W /= 1000d; // grams to kg

            double intWeight = (int)(W); 
            double fractWeight = (int)((W - (intWeight)) * 1000d)*1000d;
            
            double effWeight = (Size / 100d) + (fractWeight / 10000d) + (intWeight * 100d);
            var v = Size * (Str / 1000d) * ((VelocityMultiplier/ 1000d) * (1d / effWeight));
            v = System.Math.Min(5000d, v);
            return v* (W);
        }
    }
}
