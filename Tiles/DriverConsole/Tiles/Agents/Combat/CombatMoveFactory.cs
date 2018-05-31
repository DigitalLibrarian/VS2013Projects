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
    public class CombatMoveFactory : ICombatMoveFactory
    {
        public ICombatMove AttackBodyPartWithWeapon(IAgent attacker, IAgent defender, ICombatMoveClass moveClass, IBodyPart targetBodyPart, IItem weapon)
        {
            var moveName = string.Format("{0} {1} with {2}", moveClass.Name, targetBodyPart.Name, weapon.Class.Name);

            return new CombatMove(moveClass, moveName, attacker, defender)
            {
                Weapon = weapon,
                DefenderBodyPart = targetBodyPart
            };
        }


        static ICombatMoveClass _grasp = new CombatMoveClass(
            name: "Grasp",
            meleeVerb: new Verb(
                new Dictionary<VerbConjugation, string>()
                {
                    { VerbConjugation.FirstPerson, "grab"},
                    { VerbConjugation.SecondPerson, "grab"},
                    { VerbConjugation.ThirdPerson, "grabs"},
                }, true),
            prepTime: 1,
            recoveryTime: 1
            )
        {
            IsMartialArts = true,
            IsDefenderPartSpecific = true,
            AttackerBodyStateChange = BodyStateChange.StartHold
        };

        static ICombatMoveClass _release = new CombatMoveClass(
            name: "Release",
            meleeVerb: new Verb(
                new Dictionary<VerbConjugation, string>()
                {
                    { VerbConjugation.FirstPerson, "release"},
                    { VerbConjugation.SecondPerson, "release"},
                    { VerbConjugation.ThirdPerson, "releases"},
                }, true),
            prepTime: 1,
            recoveryTime: 1
            )
        {
            IsMartialArts = true,
            AttackerBodyStateChange = BodyStateChange.ReleaseHold
        };

        static ICombatMoveClass _wrestlingPull = new CombatMoveClass(
            name: "Pull",
            meleeVerb: new Verb(
                new Dictionary<VerbConjugation, string>()
                {
                    { VerbConjugation.FirstPerson, "pull"},
                    { VerbConjugation.SecondPerson, "pull"},
                    { VerbConjugation.ThirdPerson, "pulls"},
                }, true),
            prepTime: 1,
            recoveryTime: 1
            )
        {
            IsMartialArts = true,
            IsStrike = true,
            IsDefenderPartSpecific = true
        };


        static ICombatMoveClass _breakGrasp = new CombatMoveClass(
            name: "Break grasp",
            meleeVerb: new Verb(
                new Dictionary<VerbConjugation, string>()
                {
                    { VerbConjugation.FirstPerson, "break away from"},
                    { VerbConjugation.SecondPerson, "break away from"},
                    { VerbConjugation.ThirdPerson, "breaks away from"},
                }, true),
            prepTime: 1,
            recoveryTime: 1
            )
        {
            IsMartialArts = true,
            IsDefenderPartSpecific = true,
            AttackerBodyStateChange = BodyStateChange.BreakHold
        };


        public ICombatMove BreakOpponentGrasp(IAgent attacker, IAgent defender, IBodyPart attackerBodyPart, IBodyPart defenderBodyPart)
        {
            var moveName = string.Format("Break {0}'s {1} grasp on {2}", defender.Name,  defenderBodyPart.Name, attackerBodyPart.Name);

            return new CombatMove(_breakGrasp, moveName, attacker, defender)
            {
                AttackerBodyPart = attackerBodyPart,
                DefenderBodyPart = defenderBodyPart
            };
        }

        public ICombatMove GraspOpponentBodyPart(IAgent attacker, IAgent defender, IBodyPart attackerBodyPart, IBodyPart defenderBodyPart)
        {
            var moveName = string.Format("Grab {0} with your {1}", defenderBodyPart.Name, attackerBodyPart.Name);

            return new CombatMove(_grasp, moveName, attacker, defender)
            {
                AttackerBodyPart = attackerBodyPart,
                DefenderBodyPart = defenderBodyPart
            };
        }

        public ICombatMove PullGraspedBodyPart(IAgent attacker, IAgent defender, IBodyPart attackerBodyPart, IBodyPart defenderBodyPart)
        {
            var moveName = string.Format("Pull {0} with your {1}", defenderBodyPart.Name, attackerBodyPart.Name);
            return new CombatMove(_wrestlingPull, moveName, attacker, defender)
            {
                AttackerBodyPart = attackerBodyPart,
                DefenderBodyPart = defenderBodyPart
            };
        }


        public ICombatMove ReleaseGraspedPart(IAgent attacker, IAgent defender, IBodyPart attackerBodyPart, IBodyPart defenderBodyPart)
        {
            var moveName = string.Format("Release {0} with your {1}", defenderBodyPart.Name, attackerBodyPart.Name);
            return new CombatMove(_release, moveName, attacker, defender)
            {
                AttackerBodyPart = attackerBodyPart,
                DefenderBodyPart = defenderBodyPart
            };
        }


        public ICombatMove BodyMove(IAgent attacker, IAgent defender, ICombatMoveClass moveClass,IBodyPart defenderBodyPart)
        {
            var moveName = string.Format("{0} {1}", 
                moveClass.Verb.Conjugate(VerbConjugation.SecondPerson), 
                defenderBodyPart.Name);

            return new CombatMove(moveClass, moveName, attacker, defender)
            {
                DefenderBodyPart = defenderBodyPart
            };
        }
    }
}
