using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Math;

namespace Tiles.Agents.Combat.CombatEvolutions
{
    public class CombatEvolution_MartialArtsStrike : CombatEvolution
    {
        public CombatEvolution_MartialArtsStrike(IActionReporter reporter, IDamageCalc damageCalc, IAgentReaper reaper) 
            : base(reporter, damageCalc, reaper) { }

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
            
            if (isWeaponBased)
            {
                dmg = DamageCalc.MeleeStrikeMoveDamage(move.Class, attacker, defender, move.DefenderBodyPart, move.Weapon);
            }
            else
            {
                dmg = DamageCalc.MeleeStrikeBodyPartAttackDamage(move.Class, attacker, defender, move.AttackerBodyPart, move.DefenderBodyPart);
            }

            var shedPart = defender.Body.DamagePart(move.DefenderBodyPart, dmg);
            bool targetPartWasShed = shedPart != null;
            if (targetPartWasShed)
            {
                HandleShedPart(attacker, defender, move, shedPart);
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
