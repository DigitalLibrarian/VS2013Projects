using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Agents.Combat
{
    public class CombatMoveClass : ICombatMoveClass
    {
        public CombatMoveClass(string name, IVerb meleeVerb, DamageVector damage)
        {
            Name = name;
            Verb = meleeVerb;
            DamageVector = damage;
        }


        // New stuff
        public string Name { get; set; }
        public IVerb Verb { get; set; }
        public DamageVector DamageVector { get; set; }
        
        public BodyStateChange AttackerBodyStateChange { get; set; }

        public bool IsDefenderPartSpecific { get; set; }

        public bool IsMartialArts { get; set; }

        public bool IsStrike { get; set; }

        public bool IsItem { get; set; }
    }
}
