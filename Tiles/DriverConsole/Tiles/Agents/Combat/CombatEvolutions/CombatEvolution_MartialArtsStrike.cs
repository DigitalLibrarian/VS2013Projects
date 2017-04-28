using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Bodies.Injuries;
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
            var armorItems = session.Defender.Outfit
                .GetItems(move.DefenderBodyPart).Where(x => x.IsArmor);

            IInjuryReport report = InjuryReportCalc.CalculateMaterialStrike(
                armorItems,
                move.Class.StressMode,
                momentum,
                move.Class.ContactArea,
                move.Class.MaxPenetration,
                move.Defender.Body,
                move.DefenderBodyPart,
                weaponMat,
                move.Sharpness);

            // TODO - need better way to give injury
            foreach (var bpInjury in report.BodyPartInjuries)
            {
                bpInjury.BodyPart.Damage.Add(bpInjury.GetTotal());
            }

            session.InjuryReport = report;

            bool targetPartWasShed = false;
            foreach (var sever in report.BodyPartInjuries.Where(x => x.Class.IsSever))
            {
                defender.Body.Amputate(sever.BodyPart);
                HandleShedPart(attacker, defender, move, sever.BodyPart);
                if (sever.BodyPart == move.DefenderBodyPart)
                {
                    targetPartWasShed = true;
                }
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
