using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Injuries;
using Tiles.Items;

namespace Tiles
{
    public class ActionReporter : IActionReporter
    {
        public IActionLog Log { get; private set; }
        public ActionReporter(IActionLog log)
        {
            Log = log;
        }

        public void ReportGrabStartBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee)
        {
            var attackerName = session.Attacker.Name;
            var defenderName = session.Defender.Name;
            var verbStr = verb.Conjugate(VerbConjugation.ThirdPerson);
            var bodyPartName = graspee.Name;

            var message = string.Format("The {0} {1} the {2}'s {3}.", attackerName, verbStr, defenderName, bodyPartName);
            Log.AddLine(message);
        }

        public void ReportGrabMiss(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee)
        {
            var attackerName = session.Attacker.Name;
            var defenderName = session.Defender.Name;
            Log.AddLine(string.Format("The {0} tries to grab the {1} but fails.", attackerName, defenderName));
        }

        public void ReportGrabReleaseBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee)
        {
            var attackerName = session.Attacker.Name;
            var defenderName = session.Defender.Name;
            var verbStr = verb.Conjugate(VerbConjugation.ThirdPerson);
            var bodyPartName = grasper.Name;

            var message = string.Format("{0} {1} the {2}'s {3}.", attackerName, verbStr, defenderName, bodyPartName);
            Log.AddLine(message);
        }

        public void ReportMeleeItemStrikeBodyPart(ICombatMoveContext session, IVerb verb, IItem item, IBodyPart bodyPart, bool targetPartWasShed)
        {
            //DebugReportCombatMove(session.Move);
            var attackerName = session.Attacker.Name;
            var defenderName = session.Defender.Name;
            var verbStr = verb.Conjugate(VerbConjugation.ThirdPerson);
            var bodyPartName = bodyPart.Name;
            var limbMessage = targetPartWasShed ? " and the severed part falls away!" : ".";
            string withWeapon = string.Format(" with it's {0}", item.Class.Name);
            
            var bpInjury = session.InjuryReport.BodyPartInjuries.FirstOrDefault();
            if (bpInjury != null) 
            { 
                var completionMessage = bpInjury.GetResultPhrase();
                if (bpInjury.Class.IsSever)
                {
                    completionMessage = limbMessage;
                }

                var message = string.Format("{0} {1} the {2}'s {3}{4}{5}",
                    attackerName, verbStr, defenderName, bodyPartName, withWeapon, completionMessage);

                Log.AddLine(message);
                foreach (var addInjury in session.InjuryReport.BodyPartInjuries.Skip(1))
                {
                    ReportAdditionalBodyPartInjury(session, addInjury);
                }
            }
            else
            {
                var message = string.Format("{0} {1} the {2}'s {3}{4}{5}",
                    attackerName, verbStr, defenderName, bodyPartName, withWeapon, " but nothing happens.");
            }
        }

        public void ReportMeleeStrikeBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart bodyPart, bool targetPartWasShed)
        {
            //DebugReportCombatMove(session.Move);
            var attackerName = session.Attacker.Name;
            var defenderName = session.Defender.Name;
            var verbStr = verb.Conjugate(VerbConjugation.ThirdPerson);
            var bodyPartName = bodyPart.Name;

            if (session.InjuryReport.BodyPartInjuries.Any())
            {
                var bpInjury = session.InjuryReport.BodyPartInjuries.First();
                var completionMessage = bpInjury.GetResultPhrase();

                var message = string.Format("{0} {1} the {2}'s {3}{4}",
                    attackerName, verbStr, defenderName, bodyPartName, completionMessage);

                Log.AddLine(message);
                foreach (var addInjury in session.InjuryReport.BodyPartInjuries.Skip(1))
                {
                    ReportAdditionalBodyPartInjury(session, addInjury);
                }
            } 
            else {
                var message = string.Format("{0} {1} the {2}'s {3} but the attack glances away.",
                    attackerName, verbStr, defenderName, bodyPartName);

                Log.AddLine(message);
            }
        }

        public void ReportDeath(IAgent agent)
        {
            var message = string.Format("The {0} is struck down!", agent.Name);
            Log.AddLine(message);
        }

        void ReportAdditionalBodyPartInjury(ICombatMoveContext context, IBodyPartInjury bpInjury)
        {
            var message = string.Format(" * {0}'s {1} was hit{2} ({3})",
                context.Defender.Name,
                bpInjury.BodyPart.Name,
                bpInjury.GetResultPhrase(),
                bpInjury.GetTotal());
            Log.AddLine(message);
        }

        void DebugReportCombatMove(ICombatMove move)
        {
            var message = string.Format("Type = {0}, Ca = {1}, Mp = {2}", move.Class.StressMode, move.Class.ContactArea, move.Class.MaxPenetration);
            Log.AddLine(message);
        }
    }
}
