using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;

namespace Tiles.Agents.Combat
{
    public class CombatMove : ICombatMove
    {
        public CombatMove(ICombatMoveClass attackMoveClass, string name, IAgent attacker, IAgent defender)
        {
            Class = attackMoveClass;
            Name = name;
            Attacker = attacker;
            Defender = defender;
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

        public double Sharpness
        {
            get {
                //Iron has [MAX_EDGE:10000], so a no-quality iron short sword has a sharpness of 5000
                return Attacker.GetStrikeMaterial(this).SharpnessMultiplier * 5000d;
            }
        }
    }
}
