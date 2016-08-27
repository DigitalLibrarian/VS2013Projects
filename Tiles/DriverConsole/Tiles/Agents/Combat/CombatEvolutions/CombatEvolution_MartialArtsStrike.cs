using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Bodies.Health.Injuries;
using Tiles.Math;

namespace Tiles.Agents.Combat.CombatEvolutions
{
    public class CombatEvolution_MartialArtsStrike : CombatEvolution
    {
        IInjuryCalc InjuryCalc { get; set; }
        public CombatEvolution_MartialArtsStrike(
            IInjuryCalc injuryCalc,
            IActionReporter reporter, 
            IDamageCalc damageCalc, 
            IAgentReaper reaper) 
            : base(reporter, damageCalc, reaper) 
        {
            InjuryCalc = injuryCalc;
        }

        protected override bool Should(ICombatMoveContext session)
        {
            var move = session.Move;
            return move.Class.IsMartialArts
                && move.Class.IsStrike
                && move.Class.IsDefenderPartSpecific;
        }

        protected override void Run(ICombatMoveContext session)
        {
            var move = session.Move;
            var attacker = session.Attacker;
            var defender = session.Defender;

            bool isWeaponBased = move.Class.IsItem;
            uint dmg = 0;

            bool targetPartWasShed = false;
            if (isWeaponBased)
            {
                //dmg = DamageCalc.MeleeStrikeMoveDamage(move.Class, attacker, defender, move.DefenderBodyPart, move.Weapon);
                foreach(var injury in InjuryCalc.MeleeWeaponStrike(
                    move.Class, 
                    attacker,
                    defender,
                    move.DefenderBodyPart,
                    move.Weapon))
                {
                    defender.Body.Health.Add(injury);

                    targetPartWasShed = injury.RemovesBodyPart;
                    defender.Body.Amputate(move.DefenderBodyPart);
                }
            }
            else
            {
                dmg = DamageCalc.MeleeStrikeBodyPartAttackDamage(move.Class, attacker, defender, move.AttackerBodyPart, move.DefenderBodyPart);
            }

            //var shedPart = defender.Body.DamagePart(move.DefenderBodyPart, dmg);


            if (targetPartWasShed)
            {
                HandleShedPart(attacker, defender, move, move.DefenderBodyPart);
            }

            if (isWeaponBased)
            {
                Reporter.ReportMeleeItemStrikeBodyPart(session, move.Class.Verb, move.Weapon, move.DefenderBodyPart, dmg, targetPartWasShed);
            }
            else
            {
                Reporter.ReportMeleeStrikeBodyPart(session, move.Class.Verb, move.DefenderBodyPart, dmg, targetPartWasShed);
            }

            var defenderDies = defender.IsDead;
            if (defenderDies)
            {
                HandleDeath(attacker, defender, move);
            }
        }
    }
}
