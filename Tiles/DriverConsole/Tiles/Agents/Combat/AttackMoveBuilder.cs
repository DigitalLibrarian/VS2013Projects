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
    public class AttackMoveBuilder : IAttackMoveBuilder
    {
        public IDamageCalc DamageCalc { get; private set; }
        public AttackMoveBuilder(IDamageCalc damageCalc)
        {
            DamageCalc = damageCalc;
        }
        public IAttackMove AttackBodyPartWithWeapon(IAgent attacker, IAgent defender, IAttackMoveClass moveClass, IBodyPart targetBodyPart, IItem weapon)
        {
            var dmg = DamageCalc.MeleeStrikeMoveDamage(moveClass, attacker, defender, targetBodyPart, weapon);
            var moveName = string.Format("{0} {1} with {2}", moveClass.Name, targetBodyPart.Name, weapon.WeaponClass.Name);

            return new AttackMove(moveClass, moveName, attacker, defender, targetBodyPart, dmg)
            {
                Weapon = weapon
            };
        }
    }
}
