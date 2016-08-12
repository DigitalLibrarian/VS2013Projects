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

            return new AttackMove(moveClass, moveName, attacker, defender, dmg)
            {
                Weapon = weapon,
                DefenderBodyPart = targetBodyPart
            };
        }


        static IAttackMoveClass _grasp = new AttackMoveClass(
            name: "Grasp",
            meleeVerb: new Verb(
                    firstPerson: "grab",
                    secondPerson: "grab",
                    thirdPerson: "grabs"),
            damage: new DamageVector()
            )
        {
            IsMeleeStrike = false,
            IsGraspPart = true
        };

        static IAttackMoveClass _release = new AttackMoveClass(
            name: "Release",
            meleeVerb: new Verb(
                    firstPerson: "release",
                    secondPerson: "release",
                    thirdPerson: "release"),
            damage: new DamageVector()
            )
        {
            IsMeleeStrike = false,
            IsReleasePart = true
        };

        static IAttackMoveClass _wrestlingPull = new AttackMoveClass(
            name: "Pull",
            meleeVerb: new Verb(
                    firstPerson: "pull",
                    secondPerson: "pull",
                    thirdPerson: "pulls"),
            damage: new DamageVector(new Dictionary<DamageType, uint>{
                    {DamageType.Blunt, 20}
                })
            )
        {
            IsMeleeStrike = false,
            TakeDamageProducts = true
        };


        public IAttackMove GraspOpponentBodyPart(IAgent attacker, IAgent defender, IBodyPart attackerBodyPart, IBodyPart defenderBodyPart)
        {
            var moveName = string.Format("Grab {0} with your {1}", defenderBodyPart.Name, attackerBodyPart.Name);

            return new AttackMove(_grasp, moveName, attacker, defender, 0)
            {
                AttackerBodyPart = attackerBodyPart,
                DefenderBodyPart = defenderBodyPart
            };
        }

        public IAttackMove PullGraspedBodyPart(IAgent attacker, IAgent defender, IBodyPart attackerBodyPart, IBodyPart defenderBodyPart)
        {
            var moveName = string.Format("Pull {0} with your {1}", defenderBodyPart.Name, attackerBodyPart.Name);
            uint dmg = 20; //TODO - calculate
            return new AttackMove(_wrestlingPull, moveName, attacker, defender, dmg)
            {
                AttackerBodyPart = attackerBodyPart,
                DefenderBodyPart = defenderBodyPart
            };
        }


        public IAttackMove ReleaseGraspedPart(IAgent attacker, IAgent defender, IBodyPart attackerBodyPart, IBodyPart defenderBodyPart)
        {
            var moveName = string.Format("Release {0} with your {1}", defenderBodyPart.Name, attackerBodyPart.Name);
            uint dmg = 0; //TODO - calculate
            return new AttackMove(_release, moveName, attacker, defender, dmg)
            {
                AttackerBodyPart = attackerBodyPart,
                DefenderBodyPart = defenderBodyPart
            };
        }
    }
}
