using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Combat
{
    public class AttackMoveClass : IAttackMoveClass
    {
        public string Name { get; private set; }
        public IVerb Verb { get; private set; }
        public DamageVector DamageVector { get; private set; }
        public bool IsMeleeStrike { get; private set; }
        public bool IsGraspPart { get; private set; }
        public bool IsReleasePart { get; private set; }
        public bool TakeDamageProducts { get; private set; }

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
