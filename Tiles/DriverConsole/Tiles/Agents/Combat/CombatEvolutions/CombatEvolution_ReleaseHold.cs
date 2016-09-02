using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Math;

namespace Tiles.Agents.Combat.CombatEvolutions
{
    public class CombatEvolution_ReleaseHold : CombatEvolution
    {
        public CombatEvolution_ReleaseHold(IActionReporter reporter, IAgentReaper reaper) 
            : base(reporter, reaper) { }

        protected override bool Should(ICombatMoveContext session)
        {
            var move = session.Move;
            return move.Class.AttackerBodyStateChange == BodyStateChange.ReleaseHold
                && move.Class.IsMartialArts
                && move.AttackerBodyPart != null
                && move.DefenderBodyPart != null
                && move.AttackerBodyPart.Grasped == move.DefenderBodyPart
                && move.DefenderBodyPart.GraspedBy == move.AttackerBodyPart;
        }

        protected override void Run(ICombatMoveContext session)
        {
            var move = session.Move;
            var attacker = session.Attacker;
            var defender = session.Defender;
            var grasper = move.AttackerBodyPart;
            var graspee = move.DefenderBodyPart;

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
