using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Math;
using Tiles.Materials;

namespace Tiles.Agents.Combat
{
    public class CombatMove : ICombatMove
    {
        public CombatMove(ICombatMoveClass attackMoveClass, string name, IAgent attacker, IAgent defender, Vector3 direction)
        {
            Class = attackMoveClass;
            Name = name;
            Attacker = attacker;
            Defender = defender;
            Direction = direction;
        }

        public ICombatMoveClass Class { get; set; }
        public string Name { get; set; }
        public string Verb { get; set; }
        public bool IsCritical { get; set; }
        public IAgent Attacker { get; set; }
        public IAgent Defender { get; set; }
        public IBodyPart AttackerBodyPart { get; set; }
        public IBodyPart DefenderBodyPart { get; set; }
        public IItem Weapon { get; set; }
        public Vector3 Direction { get; set; }

        public double Sharpness
        {
            get {
                //Iron has [MAX_EDGE:10000], so a no-quality iron short sword has a sharpness of 5000
                var strikeMat = GetStrikeMaterial();
                return strikeMat.SharpnessMultiplier * 5000d;
            }
        }

        public bool IsDodged { get; private set; }
        public void MarkDodged()
        {
            IsDodged = true;
        }

        public bool CanPerform(IAgent agent)
        {
            if (this.Class.IsItem)
            {
                return agent.Outfit.IsWielded(this.Weapon);
            }
            else
            {
                if (this.Class.IsGrabRequired && !agent.Body.IsGrasping) return false;
                return this.Class.GetRelatedBodyParts(agent.Body).Any();
            }
        }

        public IMaterial GetStrikeMaterial()
        {
            IAgent agent = Attacker;
            if (this.Class.IsItem)
            {
                return this.Weapon.Class.Material;
            }
            else
            {
                var relatedParts = this.Class.GetRelatedBodyParts(agent.Body);
                var strikePart = relatedParts.First();

                var childTissueReq = this.Class.Requirements.FirstOrDefault(r => r.Type == BodyPartRequirementType.ChildTissueLayerGroup);
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
            }
        }

        public double GetStrikeMomentum()
        {
            IAgent agent = Attacker;
            double Str = agent.Body.GetAttribute("STRENGTH"),
                    VelocityMultiplier = (double)this.Class.VelocityMultiplier,
                    Size = agent.Body.Size;

            if (this.Class.IsItem)
            {
                var weapon = this.Weapon;
                var weight = weapon.GetMass();

                weight /= 1000d; // grams to kg

                double intWeight = (int)(weight);
                double fractWeight = (int)((weight - (intWeight)) * 1000d) * 1000d;

                double effWeight = (Size / 100d) + (fractWeight / 10000d) + (intWeight * 100d);
                double actWeight = (intWeight * 1000d) + (fractWeight / 1000d);

                var v = Size * (Str / 1000d) * ((VelocityMultiplier / 1000d) * (1d / effWeight));
                v = System.Math.Min(5000d, v);
                var mom = v * actWeight / 1000d + 1d;

                return mom;
            }
            else
            {
                var parts = this.Class.GetRelatedBodyParts(this.Attacker.Body);
                var strikerMat = GetStrikeMaterial();
                var density = strikerMat.SolidDensity / 100d;
                double partWeight = parts
                    .Select(p => p.Mass * (double)p.Class.Number)
                    .Sum();

                var v = 100d * (Str / 1000d) * (VelocityMultiplier / 1000d);
                return v * (partWeight / 1000) + 1;
            }
        }
    }
}
