using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Combat
{
    public class AttackMoveClass : IAttackMoveClass
    {
        public string Name { get; set; }
        public IVerb Verb { get; set; }
        public DamageVector DamageVector { get; set; }
        public bool IsMeleeStrike { get; set; }
        public bool IsGraspPart { get; set; }
        public bool IsReleasePart { get; set; }
        public bool TakeDamageProducts { get; set; }

        public AttackMoveClass(string name, IVerb meleeVerb, DamageVector damage)
        {
            Name = name;
            Verb = meleeVerb;
            DamageVector = damage;
            TakeDamageProducts = false;
            IsMeleeStrike = true;
            IsGraspPart = false;
            IsReleasePart = false;
        }
    }
}
