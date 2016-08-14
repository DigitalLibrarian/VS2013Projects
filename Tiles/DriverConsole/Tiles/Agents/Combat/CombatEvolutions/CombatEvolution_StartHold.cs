using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Math;

namespace Tiles.Agents.Combat.CombatEvolutions
{
    public class CombatEvolution_StartHold : CombatEvolution
    {
        public CombatEvolution_StartHold(IActionReporter reporter, IDamageCalc damageCalc) : base(reporter, damageCalc) { }

        protected override bool Should(ICombatMoveContext session)
        {
            var move = session.Move;
            return move.Class.AttackerBodyStateChange == BodyStateChange.StartHold
                && move.Class.IsMartialArts
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
}
