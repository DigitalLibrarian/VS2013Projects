using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Bodies.Injuries;
using Tiles.Bodies.Wounds;
using Tiles.Items;
using Tiles.Materials;
using Tiles.Math;
using Tiles.Splatter;

namespace Tiles.Agents.Combat.CombatEvolutions
{
    public class CombatEvolution_MartialArtsStrike : CombatEvolution
    {
        IAtlas Atlas { get; set; }
        IInjuryReportCalc InjuryReportCalc { get; set; }
        IBodyPartWoundFactory WoundFactory { get; set; }
        ISplatterFascade Splatter { get; set; }
        public CombatEvolution_MartialArtsStrike(
            IAtlas atlas,
            IInjuryReportCalc injuryReportCalc,
            IActionReporter reporter, 
            IAgentReaper reaper,
            IBodyPartWoundFactory woundFactory,
            ISplatterFascade splatter) 
            : base(reporter, reaper) 
        {
            Atlas = atlas;
            InjuryReportCalc = injuryReportCalc;
            WoundFactory = woundFactory;
            Splatter = splatter;
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

            if (move.IsDodged)
            {
                Reporter.ReportDodgedAttack(session);
                return;
            }

            bool isWeaponBased = move.Class.IsItem;
            var momentum = attacker.GetStrikeMomentum(move);
            var weaponMat = attacker.GetStrikeMaterial(move);
            if (weaponMat == null) return;
            bool implementWasSmall = false;
            double implementSize;
            if (!isWeaponBased)
            {
                var relatedParts = move.Class.GetRelatedBodyParts(attacker.Body);
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

            var targetWasWoke = defender.IsWoke;
            var targetWasProne = defender.IsProne;
            foreach (var bpInjury in report.BodyPartInjuries)
            {
                defender.AddInjury(bpInjury, WoundFactory);
            }

            var targetPartWasShed = report.IsSever(move.DefenderBodyPart);
            foreach (var sever in report.GetSeverings())
            {
                defender.Sever(sever.BodyPart);
                HandleShedPart(attacker, defender, move, sever.BodyPart);
            }
            var targetIsWoke = defender.IsWoke;
            var targetIsProne = defender.IsProne;

            if (isWeaponBased)
            {
                Reporter.ReportMeleeItemStrikeBodyPart(session, move.Class.Verb, move.Weapon, move.DefenderBodyPart, targetPartWasShed);
            }
            else
            {
                Reporter.ReportMeleeStrikeBodyPart(session, move.Class.Verb, move.DefenderBodyPart, targetPartWasShed);
            }

            if (report.BodyPartInjuries.SelectMany(bpi => bpi.TissueLayerInjuries).Any(tli => tli.ArteryOpened))
            {
                Splatter.ArterySquirt(defender.Pos, defender.Body.Class.BloodMaterial, 2);
                Reporter.ReportArteryOpened(defender);
            }

            if (report.BodyPartInjuries.SelectMany(bpi => bpi.TissueLayerInjuries).Any(tli => tli.MajorArteryOpened))
            {
                Splatter.ArterySquirt(defender.Pos, defender.Body.Class.BloodMaterial, 3);
                Reporter.ReportMajorArteryOpened(defender);
            }

            if (!targetWasProne && targetIsProne)
            {
                Reporter.ReportFallDown(session, move.Defender);
            }

            if (targetWasWoke && !targetIsWoke && !defender.CanWake())
            {
                Reporter.ReportGiveInToPain(session, defender);
            }

            var defenderDies = defender.IsDead;
            if (defenderDies)
            {
                HandleDeath(attacker, defender, move);
            }
        }
    }
}
