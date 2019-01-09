using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Bodies.Injuries;
using Tiles.Items;
using Tiles.Materials;

namespace Tiles
{
    public class ActionReporter : IActionReporter
    {
        // TODO - need proper name awareness

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

            var message = string.Format("The {0} {1} the {2}'s {3}.", attackerName, verbStr, defenderName, bodyPartName);
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
                var completionMessage = GetPhrase(bpInjury);
                if (bpInjury.IsSever)
                {
                    completionMessage = limbMessage;
                }

                var message = string.Format("The {0} {1} the {2}'s {3}{4}{5}",
                    attackerName, verbStr, defenderName, bodyPartName, withWeapon, completionMessage);

                Log.AddLine(message);
                foreach (var addInjury in session.InjuryReport.BodyPartInjuries.Skip(1))
                {
                    ReportAdditionalBodyPartInjury(session, addInjury);
                }
            }
            else
            {
                var message = string.Format("The {0} {1} the {2}'s {3}{4}{5}",
                    attackerName, verbStr, defenderName, bodyPartName, withWeapon, " but nothing happens.");

                Log.AddLine(message);
            }
        }

        public void ReportMeleeStrikeBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart bodyPart, bool targetPartWasShed)
        {
            //DebugReportCombatMove(session.Move);
            var attackerName = session.Attacker.Name;
            var defenderName = session.Defender.Name;
            var verbStr = verb.Conjugate(VerbConjugation.ThirdPerson);
            var bodyPartName = bodyPart.Name;
            var limbMessage = targetPartWasShed ? " and the severed part falls away!" : ".";

            if (session.InjuryReport.BodyPartInjuries.Any())
            {
                var bpInjury = session.InjuryReport.BodyPartInjuries.First();
                var completionMessage = GetPhrase(bpInjury);
                if (bpInjury.IsSever)
                {
                    completionMessage = limbMessage;
                }

                var message = string.Format("The {0} {1} the {2}'s {3}{4}",
                    attackerName, verbStr, defenderName, bodyPartName, completionMessage);

                Log.AddLine(message);
                foreach (var addInjury in session.InjuryReport.BodyPartInjuries.Skip(1))
                {
                    ReportAdditionalBodyPartInjury(session, addInjury);
                }
            } 
            else {
                var message = string.Format("The {0} {1} the {2}'s {3} but the attack glances away.",
                    attackerName, verbStr, defenderName, bodyPartName);

                Log.AddLine(message);
            }
        }

        public void ReportDeath(IAgent agent)
        {
            var message = string.Format("The {0} is struck down!", agent.Name);
            Log.AddLine(message);
        }
        
        public void ReportGiveInToPain(ICombatMoveContext session, Agents.IAgent agent)
        {
            Log.AddLine(string.Format("The {0} gives in to pain.", agent.Name));
        }

        public void ReportFallDown(ICombatMoveContext session, IAgent agent)
        {
            Log.AddLine(string.Format("The {0} falls down.", agent.Name));
        }

        void ReportAdditionalBodyPartInjury(ICombatMoveContext context, IBodyPartInjury bpInjury)
        {
            var message = string.Format(" The {0}'s {1} was hit{2}",
                context.Defender.Name,
                bpInjury.BodyPart.Name,
                GetPhrase(bpInjury));
            Log.AddLine(message);
        }

        void DebugReportCombatMove(ICombatMove move)
        {
            var message = string.Format("Type = {0}, Ca = {1}, Mp = {2}", move.Class.StressMode, move.Class.ContactArea, move.Class.MaxPenetration);
            Log.AddLine(message);
        }

        string GetGerund(ITissueLayerInjury tlInjury)
        {
            var gerund = "";
            switch (tlInjury.StressResult)
            {
                case StressResult.None:
                    gerund = "glancing off";
                    break;
                case StressResult.Impact_Dent:
                    gerund = tlInjury.IsVascular ? "bruising" : "denting";
                    break;
                case StressResult.Impact_Bypass:
                    gerund = tlInjury.IsVascular ? "bruising" : "denting";
                    break;
                case StressResult.Impact_InitiateFracture:
                    if (tlInjury.IsChip) gerund = "chipping";
                    else
                    {
                        gerund = tlInjury.IsSoft ? "tearing" : "fracturing";
                    }
                    break;
                case StressResult.Impact_CompleteFracture:
                    if (tlInjury.IsChip) gerund = "chipping";
                    else
                    {
                        gerund = tlInjury.IsSoft ? "tearing apart" : "shattering";
                    }
                    break;
                case StressResult.Shear_Dent:
                    gerund = tlInjury.IsVascular ? "bruising" : "denting";
                    break;
                case StressResult.Shear_Cut:
                    if (!tlInjury.IsSoft)
                    {
                        if (tlInjury.IsChip) gerund = "chipping";
                        else
                        {
                            gerund = "fracturing";
                        }
                    }
                    else
                    {
                        gerund = "tearing";
                    }
                    break;
                case StressResult.Shear_CutThrough:

                    if (tlInjury.IsSoft)
                    {
                        gerund = "tearing apart";
                    }
                    else
                    {
                        gerund = tlInjury.IsChip ? "tearing through" : "shattering";
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            return gerund;
        }

        string GetPhrase(ITissueLayerInjury tlInjury)
        {
            var gerund = GetGerund(tlInjury);
            return string.Format("{0} the {1}", gerund, tlInjury.Layer.Class.Name);
        }

        string GetPhrase(IBodyPartInjury bpInjury)
        {
            var injuries = bpInjury.TissueLayerInjuries;
            if (injuries.Count() > 1)
            {
                injuries = bpInjury.TissueLayerInjuries.Where(x => x.StressResult != Materials.StressResult.None);
            }

            if (injuries.Any())
            {
                var phrases = injuries
                    .Select(injury =>
                    {
                        var remaining = injuries.SkipWhile(x => x != injury);
                        var grouped = remaining.TakeWhile(x => GetGerund(x).Equals(GetGerund(injury)));
                        return grouped.Last() == injury ? injury : null;
                    })
                    .Where(x => x != null)
                    .Select(x => GetPhrase(x))
                    .ToList();
                if (phrases.Count() > 1)
                {
                    var last = phrases.Last();
                    phrases[phrases.Count() - 1] = string.Format("and {0}", last);
                }
                return string.Format(", {0}!", string.Join(", ", phrases));
            }
            else
            {
                return ", but the attack glances away.";
            }
        }

        public void ReportBledOut(IAgent agent)
        {
            Log.AddLine(string.Format("The {0} has bled out.", agent.Name));
        }

        public void ReportArteryOpened(IAgent agent)
        {
            Log.AddLine("An artery was opened by the attack!");
        }
    }
}
