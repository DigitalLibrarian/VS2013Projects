using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Agents.Combat
{
    public enum ContactType 
    {
        None,
        Edge,
        Blunt,
        Point
    }

    public class CombatMoveClass : ICombatMoveClass
    {
        public CombatMoveClass(string name, 
            IVerb meleeVerb, 
            IDamageVector damage, 
            int prepTime, int recoveryTime)
        {
            Name = name;
            Verb = meleeVerb;
            DamageVector = damage;
        }


        // New stuff
        public string Name { get; set; }
        public IVerb Verb { get; set; }
        public IDamageVector DamageVector { get; set; }
        public int PrepTime { get; set; }
        public int RecoveryTime { get; set; }
        
        public BodyStateChange AttackerBodyStateChange { get; set; }

        public bool IsDefenderPartSpecific { get; set; }

        public bool IsMartialArts { get; set; }

        public bool IsStrike { get; set; }

        public bool IsItem { get; set; }

        public ContactType ContactType { get; set; }
        public int ContactArea { get; set; }
        public int MaxPenetration { get; set; }
        public int VelocityMultiplier { get; set; }
    }
}
