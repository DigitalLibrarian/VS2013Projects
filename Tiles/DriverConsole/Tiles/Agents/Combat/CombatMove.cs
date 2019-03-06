using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Math;

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
                var strikeMat = Attacker.GetStrikeMaterial(this);
                return strikeMat.SharpnessMultiplier * 5000d;
            }
        }

        public bool IsDodged { get; private set; }
        public void MarkDodged()
        {
            IsDodged = true;
        }
    }
}
