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
        public bool IsMeleeStrike { get; set; }
        public bool IsGraspPart { get; set; }
        public bool IsReleasePart { get; set; }
        public bool TakeDamageProducts { get; set; }

        public CombatMoveClass(string name, IVerb meleeVerb, DamageVector damage)
        {
            Name = name;
            Verb = meleeVerb;
            DamageVector = damage;
            TakeDamageProducts = false;
            IsMeleeStrike = true;
            IsGraspPart = false;
            IsReleasePart = false;
            IsGraspBreak = false;
        }

        public bool IsGraspBreak { get; set; }

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
