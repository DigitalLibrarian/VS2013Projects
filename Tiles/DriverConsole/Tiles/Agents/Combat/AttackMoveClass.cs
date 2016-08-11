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
        public IVerb MeleeVerb { get; private set; }
        public DamageVector DamageVector { get; private set; }

        public AttackMoveClass(string name, IVerb meleeVerb, DamageVector damage)
        {
            Name = name;
            MeleeVerb = meleeVerb;
            DamageVector = damage;
        }
    }
}
