using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Bodies.Health.Injuries;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Agents.Combat.CombatEvolutions
{
    public class CombatEvolution_MartialArtsStrike : CombatEvolution
    {
        IInjuryCalc InjuryCalc { get; set; }
        public CombatEvolution_MartialArtsStrike(
            IInjuryCalc injuryCalc,
            IActionReporter reporter, 
            IAgentReaper reaper) 
            : base(reporter, reaper) 
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

            var momentum = GetStrikeMomentum(attacker, move);

            bool targetPartWasShed = false;
            IEnumerable<IInjury> injuries = Enumerable.Empty<IInjury>();
            if (isWeaponBased)
            {
                injuries = InjuryCalc.MeleeWeaponStrike(
                    move.Class,
                    momentum,
                    attacker,
                    defender,
                    move.DefenderBodyPart,
                    move.Weapon);
            }
            else
            {
                injuries = InjuryCalc.UnarmedStrike(
                    move.Class,
                    momentum,
                    attacker,
                    defender,
                    move.DefenderBodyPart);
            }

            session.NewInjuries.AddRange(injuries);

            bool partRemoveSuccess = false;
            foreach (var injury in injuries)
            {
                defender.Body.Health.Add(injury);

                targetPartWasShed = injury.RemovesBodyPart;
                if (targetPartWasShed && defender.Body.Parts.Contains(move.DefenderBodyPart))
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
                Reporter.ReportMeleeItemStrikeBodyPart(session, move.Class.Verb, move.Weapon, move.DefenderBodyPart, partRemoveSuccess);
            }
            else
            {
                Reporter.ReportMeleeStrikeBodyPart(session, move.Class.Verb, move.DefenderBodyPart, partRemoveSuccess);
            }

            var defenderDies = defender.IsDead;
            if (defenderDies)
            {
                HandleDeath(attacker, defender, move);
            }
        }

        double GetStrikeMomentum(IAgent agent, ICombatMove move)
        {
            // M = (Str * VelocityMultiplier) / ((10^6/Size) + ((10 * F) / W)

            if (move.Class.IsItem)
            {
                var weapon = move.Weapon;
                double Str = 1250;
                double VelocityMultiplier = (double) move.Class.VelocityMultiplier / 1000d;
                double Size = 60000;
                double Fat = 1;
                double W = weapon.GetMass() / 1000d;

                return (Str * VelocityMultiplier)
                    / ((1000000d / Size) + ((10 * Fat) / W));
            }
            else
            {
                // TODO 
                return 50;
            }
        }
    }
}
