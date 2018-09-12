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

            if (!defender.Body.Parts.Contains(move.DefenderBodyPart)) return;
            if (!attacker.CanPerform(move)) return;

            bool isWeaponBased = move.Class.IsItem;
            var momentum = attacker.GetStrikeMomentum(move);
            var weaponMat = attacker.GetStrikeMaterial(move);
            if (weaponMat == null) return;
            bool implementWasSmall = false;
            double implementSize;
            if (!isWeaponBased)
            {
                var relatedParts = move.Class.GetRelatedBodyParts(defender.Body);
                implementWasSmall = relatedParts.All(x => x.Class.IsSmall);
                implementSize = relatedParts.Sum(x => x.Size);
            }
            else
            {
                implementSize = move.Weapon.Class.Size;
            }

            var armorItems = session.Defender.Outfit
                .GetItems(move.DefenderBodyPart).Where(x => x.IsArmor);
            var report = InjuryReportCalc.CalculateMaterialStrike(
                armorItems,
                move.Class.StressMode,
                momentum,
                move.Class.ContactArea,
                move.Class.MaxPenetration,
                move.Defender.Body,
                move.DefenderBodyPart,
                weaponMat,
                move.Sharpness,
                implementWasSmall,
                implementSize);

            session.InjuryReport = report;

            var targetWasProne = defender.IsProne;
            
            foreach (var bpInjury in report.BodyPartInjuries)
            {
                defender.AddInjury(bpInjury);
            }

            var targetIsProne = false;
            var targetPartWasShed = report.IsSever(move.DefenderBodyPart);
            foreach (var sever in report.GetSeverings())
            {
                defender.Sever(sever.BodyPart);
                HandleShedPart(attacker, defender, move, sever.BodyPart);
            }
            targetIsProne = defender.IsProne;

            if (isWeaponBased)
            {
                Reporter.ReportMeleeItemStrikeBodyPart(session, move.Class.Verb, move.Weapon, move.DefenderBodyPart, targetPartWasShed);
            }
            else
            {
                Reporter.ReportMeleeStrikeBodyPart(session, move.Class.Verb, move.DefenderBodyPart, targetPartWasShed);
            }

            if (!targetWasProne && targetIsProne)
            {
                Reporter.ReportFallDown(session, move.Defender);
            }

            var defenderDies = defender.IsDead;
            if (defenderDies)
            {
                HandleDeath(attacker, defender, move);
            }
        }
    }
}
