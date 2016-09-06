using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Bodies.Health.Injuries;
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
            var attackerName = session.Attacker.Name;
            var defenderName = session.Defender.Name;
            var verbStr = verb.Conjugate(VerbConjugation.ThirdPerson);
            var bodyPartName = bodyPart.Name;
            var limbMessage = targetPartWasShed ? " and the severed part falls away!" : ".";
            string withWeapon = string.Format(" with it's {0}", item.Class.Name);
            Log.AddLine(string.Format("{0} {1} the {2}'s {3}{4}{5}", attackerName, verbStr, defenderName, bodyPartName, withWeapon, limbMessage));

            foreach (var injury in session.NewInjuries)
            {
                ReportInjury(session.Defender, injury);
            }
        }

        public void ReportMeleeStrikeBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart bodyPart, bool targetPartWasShed)
        {
            var attackerName = session.Attacker.Name;
            var defenderName = session.Defender.Name;
            var verbStr = verb.Conjugate(VerbConjugation.ThirdPerson);
            var bodyPartName = bodyPart.Name;
            var limbMessage = targetPartWasShed ? " and it comes off!" : ".";

            var message = string.Format("{0} {1} the {2}'s {3}{4}", attackerName, verbStr, defenderName, bodyPartName, limbMessage);
            Log.AddLine(message);

            foreach (var injury in session.NewInjuries)
            {
                ReportInjury(session.Defender, injury);
            }
        }

        void ReportInjury(IAgent defender, IInjury injury)
        {
            var message = string.Format("It is {0}! ({1})", injury.Adjective, injury.Damage);
            Log.AddLine(message);
        }

        public void ReportDeath(IAgent agent)
        {
            var message = string.Format("The {0} is struck down!", agent.Name);
            Log.AddLine(message);
        }
    }
}
