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
using Tiles.Bodies.Injuries;
using Tiles.Bodies.Wounds;
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
        public bool IsWoke { get; set; }
        public bool CanStand { get { return !CantStand(); } }
        public bool IsDead { get { return Body.IsDead; } }

        public IAgentCommandQueue CommandQueue { get; private set; }

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
            IsWoke = CanWake();
            IsProne = CantStand();
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

        public bool CanWake()
        {
            if (Body.Class.FeelsNoPain) return true;
            return Body.TotalPain < GetPainThreshold();
        }

        // TODO - Belongs in CombatMove
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

                var childTissueReq = move.Class.Requirements.FirstOrDefault(r => r.Type == BodyPartRequirementType.ChildTissueLayerGroup);
                if (childTissueReq != null)
                {
                    // TODO - We really should be keeping the reference name 
                    // from the raws.  It can be used to drive this.
                    var tokens = childTissueReq.Constraints.Last().Tokens;
                    var layerName = tokens.Last().ToLower();

                    var tissue = strikePart.Tissue.TissueLayers.First(tl => tl.Class.Name.ToLower().Equals(layerName));
                    return tissue.Material;
                }


                return strikePart.Tissue.TissueLayers.Select(x => x.Material).Last();

                return strikePart.Tissue.TissueLayers.Select(x => x.Material)
                    .OrderByDescending(x => x.ShearFracture)
                    .First();
            }
        }

        private double MeleeWeaponMomentum(double skill, double creatureClassSize, double strength,
            double velocityMultipier, double creatureSize, double weaponDensity, double weaponSize)
        {
            return (skill * creatureClassSize * strength * velocityMultipier)
                 / (1000000d * (1d + creatureSize / (weaponDensity * weaponSize)));
        }

        // TODO - Belongs in CombatMove
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
                var mom =  v * actWeight / 1000d + 1d;

                return mom;
            }
            else
            {
                var parts = move.Class.GetRelatedBodyParts(move.Attacker.Body);
                var strikerMat = GetStrikeMaterial(move);
                var density = strikerMat.SolidDensity / 100d;
                double partWeight = parts
                    .Select(p => p.Mass * (double) p.Class.Number)
                    .Sum();

                //var weight = partWeight;
                //weight /= 1000d; // grams to kg

                //double intWeight = (int)(weight);
                //double fractWeight = (int)((weight - (intWeight)) * 1000d) * 1000d;

                //double effWeight = (Size / 100d) + (fractWeight / 10000d) + (intWeight * 100d);
                //double actWeight = (intWeight * 1000d) + (fractWeight / 1000d);

                //var v = Size * (Str / 1000d) * ((VelocityMultiplier / 1000d) * (1d / effWeight));
                //v = System.Math.Min(5000d, v);
                //return v * actWeight / 1000d + 1d;

                var v = 100d * (Str / 1000d) * (VelocityMultiplier / 1000d);
                return v * (partWeight / 1000) + 1;
            }
        }

        public void Sever(IBodyPart part)
        {
            Body.Amputate(part);
            UpdateIsProne();
        }

        private void UpdateIsProne()
        {
            if (!IsProne)
            {
                IsProne = CantStand();
            }
        }

        private void UpdateIsWoke()
        {
            IsWoke = IsWoke && CanWake();
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

            return !CanWake();
        }

        public bool StandUp()
        {
            if (!IsProne) return false;
            if (!CanStand) return false;

            IsProne = false;
            return true;
        }

        public bool LayDown()
        {
            if (IsProne) return false;

            IsProne = true;
            return true;
        }

        public int GetPainThreshold()
        {
            return Body.GetAttribute("WILLPOWER") / 10;
        }

        public void AddInjury(IBodyPartInjury injury, IBodyPartWoundFactory woundFactory)
        {
            Body.AddInjury(injury, woundFactory);
            UpdateIsWoke();
            UpdateIsProne();
        }
    }
}
