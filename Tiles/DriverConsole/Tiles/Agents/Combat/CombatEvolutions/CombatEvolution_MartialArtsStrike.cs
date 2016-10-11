using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Injuries;
using Tiles.Items;
using Tiles.Materials;
using Tiles.Math;

namespace Tiles.Agents.Combat.CombatEvolutions
{
    public class CombatEvolution_MartialArtsStrike : CombatEvolution
    {
        IInjuryReportCalc InjuryReportCalc { get; set; }
        public CombatEvolution_MartialArtsStrike(
            IInjuryReportCalc injuryReportCalc,
            IActionReporter reporter, 
            IAgentReaper reaper) 
            : base(reporter, reaper) 
        {
            InjuryReportCalc = injuryReportCalc;
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

            var momentum = attacker.GetStrikeMomentum(move);

            IMaterial weaponMat = attacker.GetStrikeMaterial(move);

            bool targetPartWasShed = false;
            IInjuryReport report = InjuryReportCalc.CalculateMaterialStrike(
                    session,
                    move.Class.StressMode,
                    momentum,
                    move.Class.ContactArea,
                    move.Class.MaxPenetration,
                    move.DefenderBodyPart,
                    weaponMat
                    );

            foreach (var bpInjury in report.BodyPartInjuries)
            {
                bpInjury.BodyPart.Damage.Add(bpInjury.GetTotal());
            }

            targetPartWasShed = report.BodyPartInjuries.Any(x => x.Class.IsSever);
            session.InjuryReport = report;

            if (targetPartWasShed)
            {
                defender.Body.Amputate(move.DefenderBodyPart);
                HandleShedPart(attacker, defender, move, move.DefenderBodyPart);
            }


            if (isWeaponBased)
            {
                Reporter.ReportMeleeItemStrikeBodyPart(session, move.Class.Verb, move.Weapon, move.DefenderBodyPart, targetPartWasShed);
            }
            else
            {
                Reporter.ReportMeleeStrikeBodyPart(session, move.Class.Verb, move.DefenderBodyPart, targetPartWasShed);
            }

            var defenderDies = defender.IsDead;
            if (defenderDies)
            {
                HandleDeath(attacker, defender, move);
            }
        }

        
    }
}
