using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Math;

namespace Tiles.Agents.Combat.CombatEvolutions
{
    public class CombatEvolution_BreakHold : CombatEvolution
    {
        public CombatEvolution_BreakHold(IActionReporter reporter, IDamageCalc damageCalc, IAgentReaper reaper) :
            base(reporter, damageCalc, reaper) { }

        protected override bool Should(ICombatMoveContext session)
        {
            var move = session.Move;
            return move.Class.AttackerBodyStateChange == BodyStateChange.BreakHold
                && move.Class.IsMartialArts
                && move.Class.IsDefenderPartSpecific 
                && move.AttackerBodyPart != null
                && move.DefenderBodyPart != null
                && move.AttackerBodyPart.GraspedBy == move.DefenderBodyPart
                && move.DefenderBodyPart.Grasped == move.AttackerBodyPart;
        }

        protected override void Run(ICombatMoveContext session)
        {
            var move = session.Move;
            var attacker = session.Attacker;
            var defender = session.Defender;
            var grasper = move.DefenderBodyPart;
            var graspee = move.AttackerBodyPart;

            if (CompassVectors.IsCompassVector(attacker.Pos - defender.Pos))
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
