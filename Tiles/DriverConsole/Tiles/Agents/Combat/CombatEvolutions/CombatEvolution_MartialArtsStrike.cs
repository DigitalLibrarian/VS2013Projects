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

            if (!defender.Body.Parts.Contains(move.DefenderBodyPart))
            {
                return;
            }

            bool isWeaponBased = move.Class.IsItem;

            int strikeForce = move.Class.ContactArea; // TODO - get agent to produce

            bool targetPartWasShed = false;
            IEnumerable<IInjury> injuries = Enumerable.Empty<IInjury>();
            if (isWeaponBased)
            {
                injuries = InjuryCalc.MeleeWeaponStrike(
                    move.Class,
                    strikeForce,
                    attacker,
                    defender,
                    move.DefenderBodyPart,
                    move.Weapon);
            }
            else
            {
                injuries = InjuryCalc.UnarmedStrike(
                    move.Class,
                    strikeForce,
                    attacker,
                    defender,
                    move.DefenderBodyPart);
            }

            bool partRemoveSuccess = false;
            foreach (var injury in injuries)
            {
                defender.Body.Health.Add(injury);

                targetPartWasShed = injury.RemovesBodyPart;
                if (defender.Body.Parts.Contains(move.DefenderBodyPart))
                {
                    partRemoveSuccess = true;
                }
            }

            if (partRemoveSuccess)
            {
                HandleShedPart(attacker, defender, move, move.DefenderBodyPart);
            }

            if (partRemoveSuccess)
            {
                defender.Body.Amputate(move.DefenderBodyPart);
            }

            if (isWeaponBased)
            {
                Reporter.ReportMeleeItemStrikeBodyPart(session, move.Class.Verb, move.Weapon, move.DefenderBodyPart, dmg, partRemoveSuccess);
            }
            else
            {
                Reporter.ReportMeleeStrikeBodyPart(session, move.Class.Verb, move.DefenderBodyPart, dmg, partRemoveSuccess);
            }

            var defenderDies = defender.IsDead;
            if (defenderDies)
            {
                HandleDeath(attacker, defender, move);
            }
        }
    }
}
