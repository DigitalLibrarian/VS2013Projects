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
        public Sprite Sprite { get { return Class.Sprite; } }
        public IBody Body { get; private set; }
        public Vector3 Pos { get; protected set; }
        public IInventory Inventory { get; private set; }
        public virtual bool IsPlayer { get { return false; } }
        public IAgentBehavior AgentBehavior { get; set; }
        public IOutfit Outfit { get; private set; }
        public bool IsUndead { get; set; }
        public bool IsProne { get; set; }

        public IAgentCommandQueue CommandQueue { get; private set; }

        public bool IsDead { 
            get 
            {
                return Body.IsDead;
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

        public bool CanPerform(ICombatMove move)
        {
            if (move.Class.IsItem)
            {
                return this.Outfit.IsWielded(move.Weapon);
            }
            else
            {
                return move.Class.GetRelatedBodyParts(Body).Any();
            }

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
                var weaponMat = strikePart.Tissue.TissueLayers.Select(x => x.Material).OrderByDescending(x => x.ShearFracture).First();
                return weaponMat;
            }
        }

        public double GetStrikeMomentum(ICombatMove move)
        {
            double  Str = Body.GetAttribute("STRENGTH"),
                    VelocityMultiplier = (double)move.Class.VelocityMultiplier,
                    Size = Body.Size;

            if (move.Class.IsItem)
            {
                var weapon = move.Weapon;
                var weight = weapon.GetMass();

                weight /= 1000d; // grams to kg

                double intWeight = (int)(weight);
                double fractWeight = (int)((weight - (intWeight)) * 1000d) * 1000d;

                double effWeight = (Size / 100d) + (fractWeight / 10000d) + (intWeight * 100d);
                double actWeight = (intWeight * 1000d) + (fractWeight / 1000d);

                var v = Size * (Str / 1000d) * ((VelocityMultiplier / 1000d) * (1d / effWeight));
                v = System.Math.Min(5000d, v);
                return v * actWeight / 1000d + 1d;
            }
            else
            {
                var parts = move.Class.GetRelatedBodyParts(move.Attacker.Body);
                double partWeight = parts
                    .Select(p => p.Mass)
                    .Sum();
                double partsize = parts.Select(p => p.Size)
                    .Sum();
                VelocityMultiplier = 1000d;

                var material = GetStrikeMaterial(move);
                var sumRelSize = parts.Select(x => x.Class.RelativeSize).Sum();
                var v = 100d * Str / 1000d * VelocityMultiplier / 1000d;
                return v * (partWeight / 1000) + 1;
            }
        }

        public void Sever(IBodyPart part)
        {
            Body.Amputate(part);
            IsProne = !IsProne && CantStand();
        }

        private bool CantStand()
        {
            // We want to detect whether or not our agent should fall over.  Why this might "spontaneously" happen:
            //  * Losing the entire set of [LEFT] or [RIGHT] parts that have [STANCE] is the cause of falling prone/supine
            //  * Being unconscious

            var stanceParts = Body.Parts.Where(p => p.IsStance && !p.IsEffectivelyPulped);
            var leftStanceParts = stanceParts.Where(p => p.IsLeft);
            if (!leftStanceParts.Any()) return true;

            var rightStanceParts = stanceParts.Where(p => p.IsRight);
            if (!rightStanceParts.Any()) return true;

            // TODO - implement unconscious check

            return false;
        }
    }
}
