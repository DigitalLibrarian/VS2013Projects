using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Math;

namespace Tiles.Agents.Combat
{
    public interface ICombatMoveContext
    {
        IAgent Attacker { get; }
        IAgent Defender { get; }
        ICombatMove Move { get; }
    }

    public class CombatSession : ICombatMoveContext
    {
        public ICombatMove Move { get; set; }

        public IAgent Attacker { get; set; }
        public IAgent Defender { get; set; }
    }

    public interface ICombatEvolution
    {
        bool Evolve(ICombatMoveContext session);
    }

    public abstract class CombatEvolution : ICombatEvolution
    {
        protected IActionReporter Reporter { get; private set; }
        protected IDamageCalc DamageCalc { get; set; }
        public CombatEvolution(IActionReporter reporter, IDamageCalc damageCalc)
        {
            Reporter = reporter;
            DamageCalc = damageCalc;
        }

        protected abstract bool Should(ICombatMoveContext session);
        protected abstract void Run(ICombatMoveContext session);

        public bool Evolve(ICombatMoveContext session)
        {
            var move = session.Move;
            if (!Should(session)) return false;
            Run(session);
            return true;
        }

        #region Reaping
        protected void HandleDeath(IAgent attacker, IAgent defender, ICombatMove move)
        {
            throw new NotImplementedException();
        }

        protected void HandleShedPart(IAgent attacker, IAgent defender, ICombatMove move, IBodyPart shedPart)
        {
            throw new NotImplementedException();
        }
        #endregion
    }


    public class CombatEvolution_MartialArtsStrike : CombatEvolution
    {
        public CombatEvolution_MartialArtsStrike(IActionReporter reporter, IDamageCalc damageCalc) : base(reporter, damageCalc) { }

        protected override bool Should(ICombatMoveContext session)
        {
            var move = session.Move;
            return move.Class.IsMartialArts
                && move.Class.IsStrike;
        }

        protected override void Run(ICombatMoveContext session)
        {
            var move = session.Move;
            var attacker = session.Attacker;
            var defender = session.Defender;

            bool isPartTarget = move.Class.IsDefenderPartSpecific;
            bool isWeaponBased = move.Class.IsItem;

            if (isPartTarget)
            {
                uint dmg = 0;

                if (isWeaponBased)
                {
                    dmg = DamageCalc.MeleeStrikeMoveDamage(move.Class as IAttackMoveClass, attacker, defender, move.DefenderBodyPart, move.Item);
                }
                else
                {
                    dmg = DamageCalc.MeleeStrikeBodyPartAttackDamage(move.Class as IAttackMoveClass,
                        attacker, defender, move.AttackerBodyPart, move.DefenderBodyPart);
                }

                var shedPart = defender.Body.DamagePart(move.DefenderBodyPart, dmg);
                bool targetPartWasShed = shedPart != null;
                if (targetPartWasShed)
                {
                    HandleShedPart(attacker, defender, move, shedPart);
                }
                var defenderDies = defender.IsDead;
                if (defenderDies)
                {
                    HandleDeath(attacker, defender, move);
                }

                if (isWeaponBased)
                {
                    Reporter.ReportMeleeItemStrikeBodyPart(attacker, defender, move.Class.Verb, move.Item, move.DefenderBodyPart, dmg, targetPartWasShed);
                }
                else
                {
                    Reporter.ReportMeleeStrikeBodyPart(attacker, defender, move.Class.Verb, move.DefenderBodyPart, dmg, targetPartWasShed);
                }
            }
        }

    }

    public class CombatEvolution_StartHold : CombatEvolution
    {
        public CombatEvolution_StartHold(IActionReporter reporter, IDamageCalc damageCalc) : base(reporter, damageCalc) { }

        protected override bool Should(ICombatMoveContext session)
        {
            var move = session.Move;
            return move.Class.AttackerBodyStateChange == BodyStateChange.StartHold
                && move.AttackerBodyPart != null
                && move.DefenderBodyPart != null
                && !move.AttackerBodyPart.IsGrasping
                && !move.DefenderBodyPart.IsBeingGrasped;
        }

        protected override void Run(ICombatMoveContext session)
        {
            var move = session.Move;
            var attacker = session.Attacker;
            var defender = session.Defender;
            var grasper = move.AttackerBodyPart;
            var graspee = move.DefenderBodyPart;

            if (CompassVectors.IsCompassVector(attacker.Pos - defender.Pos) && !move.DefenderBodyPart.IsWrestling)
            {
                grasper.StartGrasp(graspee);

                Reporter.ReportGrabStartBodyPart(session, move.Class.Verb, grasper, graspee);
            }
            else
            {
                Reporter.ReportGrabMiss(session, move.Class.Verb, grasper, graspee);
            }

        }
    }


    public class CombatEvolution_ReleaseHold : CombatEvolution
    {
        public CombatEvolution_ReleaseHold(IActionReporter reporter, IDamageCalc damageCalc) : base(reporter, damageCalc) { }

        protected override bool Should(ICombatMoveContext session)
        {
            var move = session.Move;
            return move.Class.AttackerBodyStateChange == BodyStateChange.ReleaseHold
                && move.AttackerBodyPart != null
                && move.DefenderBodyPart != null
                && move.AttackerBodyPart.Grasped == move.DefenderBodyPart
                && move.DefenderBodyPart.Grasper == move.AttackerBodyPart
                && !move.DefenderBodyPart.IsWrestling;
        }

        protected override void Run(ICombatMoveContext session)
        {
            var move = session.Move;
            var attacker = session.Attacker;
            var defender = session.Defender;
            var grasper = move.AttackerBodyPart;
            var graspee = move.DefenderBodyPart;

            if (CompassVectors.IsCompassVector(attacker.Pos - defender.Pos) && !move.DefenderBodyPart.IsWrestling)
            {
                grasper.StopGrasp(graspee);

                Reporter.ReportGrabReleaseBodyPart(session, move.Class.Verb, grasper, graspee);
            }
            else
            {
                Reporter.ReportGrabMiss(session, move.Class.Verb, grasper, graspee);
            }
        }
        
    }


    public class CombatEvolution_BreakHold : CombatEvolution
    {
        public CombatEvolution_BreakHold(IActionReporter reporter, IDamageCalc damageCalc) : base(reporter, damageCalc) { }

        protected override bool Should(ICombatMoveContext session)
        {
            var move = session.Move;
            return move.Class.AttackerBodyStateChange == BodyStateChange.BreakHold
                && move.AttackerBodyPart != null
                && move.DefenderBodyPart != null
                && move.AttackerBodyPart.Grasped == move.DefenderBodyPart
                && move.DefenderBodyPart.Grasper == move.AttackerBodyPart
                && !move.DefenderBodyPart.IsWrestling;
        }

        protected override void Run(ICombatMoveContext session)
        {
            var move = session.Move;
            var attacker = session.Attacker;
            var defender = session.Defender;
            var grasper = move.AttackerBodyPart;
            var graspee = move.DefenderBodyPart;

            if (CompassVectors.IsCompassVector(attacker.Pos - defender.Pos) && !move.DefenderBodyPart.IsWrestling)
            {
                grasper.StopGrasp(graspee);

                Reporter.ReportGrabReleaseBodyPart(session, move.Class.Verb, grasper, graspee);
            }
            else
            {
                Reporter.ReportGrabMiss(session, move.Class.Verb, grasper, graspee);
            }
        }

    }
}
