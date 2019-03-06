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
            var attackerName = session.Attacker.Name;
            var defenderName = session.Defender.Name;
            var verbStr = verb.Conjugate(VerbConjugation.ThirdPerson);
            string withWeapon = item == null ? "" : string.Format(" with its {0}", item.Class.Name);
            var bodyPartName = bodyPart.Name;

            
            var bpInjury = session.InjuryReport.BodyPartInjuries.FirstOrDefault();
            if (bpInjury != null)
            {
                var message = string.Format("The {0} {1} the {2} in the {3}{4}{5}",
                    attackerName, verbStr, defenderName, bodyPartName, withWeapon, GetPhrase(session.InjuryReport));

                Log.AddLine(message);
            }
            else
            {
                var message = string.Format("The {0} {1} the {2} in the {3}{4}{5}",
                    attackerName, verbStr, defenderName, bodyPartName, withWeapon, " and the attack misses.");

                Log.AddLine(message);
            }
        }

        public void ReportMeleeStrikeBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart bodyPart, bool targetPartWasShed)
        {
            ReportMeleeItemStrikeBodyPart(session, verb, null, bodyPart, targetPartWasShed);
        }

        public void ReportDeath(IAgent agent)
        {
            Log.AddLine(string.Format("The {0} is struck down!", agent.Name));
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
        
        string GetPhrase(Tuple<IBodyPart, ITissueLayerInjury> tlInjury)
        {
            var gerund = GetGerund(tlInjury.Item2);
            if (tlInjury.Item1.Tissue.TissueLayers.Count() == 1)
            {
                return string.Format("{0} the {1}", gerund, tlInjury.Item1.Name);
            }
            return string.Format("{0} the {1}'s {2}", gerund, tlInjury.Item1.Name, tlInjury.Item2.Layer.Class.Name);
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
                return GetPhrase(injuries);
            }
            else
            {
                return ", but the attack glances away.";
            }
        }

        string GetPhrase(IEnumerable<Tuple<IBodyPart, ITissueLayerInjury>> injuries)
        {
            var phrases = GetPhrases(injuries).ToList();

            if (phrases.Count() > 1)
            {
                var last = phrases.Last();
                phrases[phrases.Count() - 1] = string.Format("and {0}", last);
            }
            return string.Format(", {0}", string.Join(", ", phrases));
        }

        IEnumerable<string> GetPhrases(IEnumerable<Tuple<IBodyPart, ITissueLayerInjury>> injuries)
        {
            var phrases = injuries
                   .Select(injury =>
                   {
                       var remaining = injuries.SkipWhile(x => x != injury);
                       var grouped = remaining.TakeWhile(x => GetGerund(x.Item2).Equals(GetGerund(injury.Item2)));
                       return grouped.Last() == injury ? injury : null;
                   })
                   .Where(x => x != null)
                   .Select(x => GetPhrase(x))
                   .ToList();

            return phrases;
        }

        string GetPhrase(IEnumerable<ITissueLayerInjury> injuries)
        {
            var phrases = GetPhrases(injuries).ToList();

            if (phrases.Count() > 1)
            {
                var last = phrases.Last();
                phrases[phrases.Count() - 1] = string.Format("and {0}", last);
            }
            return string.Format(", {0}", string.Join(", ", phrases));
        }

        IEnumerable<string> GetPhrases(IEnumerable<ITissueLayerInjury> injuries)
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
            
            return phrases;
        }

        string GetPhrase(IInjuryReport injuryReport)
        {
            if (injuryReport.IsPrimaryTargetSevered())
            {
                return " and the severed part falls away!";
            }

            string jamPhrase = null;
            IBodyPart bp1=null, bp2=null;
            ITissueLayer tl1=null, tl2=null;
            if (injuryReport.IsJam(out bp1, out tl1, out bp2, out tl2))
            {
                string jamObject = "";
                if (bp1.Tissue.TissueLayers.Count() == 1)
                {
                    jamObject = bp1.Name;
                }
                else
                {
                    jamObject = tl1.Name;
                }

                string jamTarget = "";
                if (bp2.Tissue.TissueLayers.Count() == 1)
                {
                    jamTarget = bp2.Name;
                }
                else
                {
                    jamTarget = string.Format("{0}'s {1}", bp2.Name, tl2.Name);
                }

                jamPhrase = string.Format("jamming the {0} through the {1}", jamObject, jamTarget);
            }

            var phrases = new List<string>();
            var jamFound = false;
            var inJam = false;
            var injuries = new List<ITissueLayerInjury>();
            var preJamInjuries = new List<ITissueLayerInjury>();
            var postJamInjuries = new List<Tuple<IBodyPart, ITissueLayerInjury>>();
            foreach (var bpInjury in injuryReport.BodyPartInjuries)
            {
                foreach (var tlInjury in bpInjury.TissueLayerInjuries)
                {
                    if (!jamFound && tlInjury.Layer == tl1 && jamPhrase != null)
                    {
                        inJam = true;
                    }

                    if (inJam)
                    {
                        jamFound = true;

                        if (tlInjury.Layer == tl2)
                        {
                            postJamInjuries.Add(new Tuple<IBodyPart, ITissueLayerInjury>(bpInjury.BodyPart, tlInjury));
                            inJam = false;
                        }
                    }
                    else
                    {
                        if (!jamFound)
                        {
                            preJamInjuries.Add(tlInjury);
                        }
                        else
                        {
                            postJamInjuries.Add(new Tuple<IBodyPart, ITissueLayerInjury>(bpInjury.BodyPart, tlInjury));
                        }
                    }
                }
            }

            if(preJamInjuries.Any())
                phrases.AddRange(GetPhrases(preJamInjuries));

            if (jamFound)
            {
                phrases.Add(jamPhrase);
            }
            
            if(postJamInjuries.Any())
                phrases.AddRange(GetPhrases(postJamInjuries));

            if (phrases.Count() > 1)
            {
                var last = phrases.Last();
                phrases[phrases.Count() - 1] = string.Format("and {0}", last);
            }
            return string.Format(", {0}!", string.Join(", ", phrases));
        }

        public void ReportBledOut(IAgent agent)
        {
            Log.AddLine(string.Format("The {0} has bled out.", agent.Name));
        }

        public void ReportArteryOpened(IAgent agent)
        {
            Log.AddLine("An artery has been opened by the attack!");
        }

        public void ReportMajorArteryOpened(IAgent agent)
        {
            Log.AddLine("A major artery has been opened by the attack!");
        }

        public void ReportDodgedAttack(ICombatMoveContext session)
        {
            var verb = session.Defender.IsProne ? "rolls" : "jumps";
            Log.AddLine(string.Format("The {0} attacks {1} but the {1} {2} away!", session.Attacker.Name, session.Defender.Name, verb));
        }
    }
}
