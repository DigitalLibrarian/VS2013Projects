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

        public bool CanWake()
        {
            if (Body.Class.FeelsNoPain) return true;
            return Body.TotalPain < GetPainThreshold();
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
