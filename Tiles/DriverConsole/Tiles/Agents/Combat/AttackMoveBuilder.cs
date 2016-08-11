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
        /*
        public IAttackMove WeaponStrike(IAgent attacker, IAgent defender, IBodyPart targetBodyPart, IWeapon weapon)
        {
            uint dmg = DamageCalc.MeleeStrikeMoveDamage(attacker, defender, targetBodyPart, weapon);
            var moveName = string.Format("Strike {0} with {1}", targetBodyPart.Name, weapon.WeaponClass.Name);
            return new AttackMove(moveName, attacker, defender, targetBodyPart, dmg)
            {
                Verb = weapon.WeaponClass.MeleeVerb,
                Weapon = weapon
            };
        }
        */

        public IAttackMove AttackBodyPartWithWeapon(IAgent attacker, IAgent defender, IAttackMoveClass moveClass, IBodyPart targetBodyPart, IWeapon weapon)
        {
            var dmg = DamageCalc.MeleeStrikeMoveDamage(moveClass, attacker, defender, targetBodyPart, weapon);
            var moveName = string.Format("{0} {1} with {2}", moveClass.Name, targetBodyPart.Name, weapon.WeaponClass.Name);



            return new AttackMove(moveClass, moveName, attacker, defender, targetBodyPart, dmg)
            {
                //Verb = weapon.WeaponClass.MeleeVerb,
                Weapon = weapon
            };
        }
    }
}
