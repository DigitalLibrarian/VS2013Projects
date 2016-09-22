using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DfCombatSnifferReaderApp
{
    public class SnifferLogParser : ISnifferLogParser
    {
        class ParserContext
        {
            public SnifferLogData Data { get; set; }
            public SnifferSession Session { get; set; }
            public AttackStrike Strike { get; set; }
            public Wound Wound { get; set; }
            public Weapon Weapon { get; set; }
            public ParserContext()
            {
                Data = new SnifferLogData();
                Bites = new Dictionary<string, string>();
                Latches = new Dictionary<string, string>();
            }

            public WoundBodyPart WoundBodyPart { get; set; }

            Dictionary<string, string> Bites { get; set; }
            Dictionary<string, string> Latches { get; set; }

            public void SetBiting(string attackerName, string defenderName)
            {
                Bites[attackerName] = defenderName;

                if(Latches.ContainsKey(attackerName)) Latches.Remove(attackerName);
            }

            public bool IsBiting(string attackerName, string defenderName)
            {
                if (!Bites.ContainsKey(attackerName)) return false;
                return Bites[attackerName].Equals(defenderName);
            }


            public void SetLatching(string attackerName, string defenderName)
            {
                Latches[attackerName] = defenderName;
            }

            public bool IsLatching(string attackerName, string defenderName)
            {
                if (!Latches.ContainsKey(attackerName)) return false;
                return Latches[attackerName].Equals(defenderName);
            }
        }

        public ISnifferLogData Parse(IEnumerable<string> lines)
        {
            var context = new ParserContext();
            var enumerator = lines.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var line = enumerator.Current;
                if (line.Trim().Equals("")) continue;
                switch (line)
                {
                    case SnifferTags.SessionStart:
                        HandleSessionStart(context, line, enumerator);
                        break;
                    case SnifferTags.AttackStart:
                        HandleAttackStart(context, line, enumerator);
                        break;
                    default:
                        if (IsKeyValue(SnifferTags.ReportText, line))
                        {
                            HandleReportText(context, line, enumerator);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                }
                
            }

            int sessionCount = 0;
            foreach (var session in context.Data.Sessions)
            {
                FixStrikeReportText(context, session, sessionCount);
            }

            return context.Data;
        }

        private void FixStrikeReportText(ParserContext context, SnifferSession session, int sessionCount)
        {
            var reportTexts = session.ReportTexts.Where(IsCombatText).ToList();

            foreach(var strike in session.Strikes)
            {
                int index = 0;
                while (strike.ReportText == null && index < reportTexts.Count())
                {
                    var reportText = reportTexts[index];
                    if (IsMatch(context, strike, reportText))
                    {
                        strike.ReportText = reportText;
                        HandleBiting(context, strike);
                        reportTexts.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }

                if (strike.ReportText == null)
                {
                    strike.ReportText = string.Format("{0} vs {1}", strike.KeyValues[SnifferTags.AttackerName], strike.KeyValues[SnifferTags.DefenderName]);
                }
            }

            int bra = 1;
        }

        private void HandleBiting(ParserContext context, AttackStrike strike)
        {
            var text = strike.ReportText;
            var attackerName = strike.KeyValues[SnifferTags.AttackerName];
            var defenderName = strike.KeyValues[SnifferTags.DefenderName];

            var bitePattern = string.Format("{0} bites {1} ", attackerName, defenderName);
            if (text.Contains(bitePattern))
            {
                context.SetBiting(attackerName, defenderName);
            }
            var latchPattern = string.Format("{0} latches on", attackerName);
            if (text.Contains(latchPattern))
            {
                context.SetLatching(attackerName, defenderName);
                if (!context.IsBiting(attackerName, defenderName))
                {
                    context.SetBiting(attackerName, defenderName);
                }
            }
            // TODO - check releases, those they might not actually be attack strikes
        }

        private bool IsMatch(ParserContext context, AttackStrike strike, string text)
        {

            var attackerName = strike.KeyValues[SnifferTags.AttackerName];
            var defenderName = strike.KeyValues[SnifferTags.DefenderName];

            if (!strike.Wounds.Any())
            {
                return text.StartsWith(attackerName)
                    && text.Contains(defenderName)
                    && IsWhiteList(text);
            }

            var wound = strike.Wounds.Last();
            if (wound.KeyValues[SnifferTags.Severed] == "true")
            {
                if (!text.Contains("severed")) return false;
            }
            else
            {
                if(text.Contains("severed")) return false;
            }
            var targetBp = strike.Wounds.Last().Parts.First().KeyValues[SnifferTags.BodyPartNameSingular];
            var targetBpPlural = strike.Wounds.Last().Parts.First().KeyValues[SnifferTags.BodyPartNamePlural];
            var layerName = strike.Wounds.Last().Parts.Last().Layers.Last().KeyValues[SnifferTags.TissueLayerName].ToLower();
            var material = strike.Wounds.Last().Parts.Last().Layers.Last().KeyValues[SnifferTags.Material].ToLower();

            var lastBpName = strike.Wounds.Last().Parts.Last().KeyValues[SnifferTags.BodyPartNameSingular];
            var lastBpPlural = strike.Wounds.Last().Parts.Last().KeyValues[SnifferTags.BodyPartNamePlural];
            

            var combatantRegex = string.Format("^{0} .+? {1}['| ]", attackerName, defenderName);//in the {2}", attackerName, defenderName, targetBp);
            bool isCombatText = IsCombatText(text);
            bool isCombatantRegex = Regex.IsMatch(text, combatantRegex);
            bool hasBut = Regex.IsMatch(text, ", but ");
            var result = isCombatText && isCombatantRegex && !hasBut;
            
            var layerRegex = string.Format(" the (({1}'s )|())(({0})|({1})|({2})|({3}))( collapses)*!", layerName, lastBpName, lastBpPlural, material);
            bool singleLayerDent = Regex.IsMatch(text, "((tear)|(bruis)|(shatter)|(dent)|(fractur))ing it!");
            bool lastBpCollapse = Regex.IsMatch(text, string.Format("{0} collapses", lastBpName));
            bool layerMatch = Regex.IsMatch(text, layerRegex);

            if (!isCombatText || hasBut) return false;

            if (isCombatantRegex)
            {
                if (lastBpCollapse) return true;
                
                var bodyPartRegex = string.Format(" in the (({0})|({1}))[ |,]", targetBp, targetBpPlural);

                if (Regex.IsMatch(text, bodyPartRegex))
                {
                    if (layerMatch || IsWhiteList(text) || singleLayerDent)
                    {
                        return true;
                    }   
                }

                /*
                bodyPartRegex = string.Format(" in the {0}[ |,]", targetBp);

                if (Regex.IsMatch(text, bodyPartRegex))
                {
                    if (layerMatch || IsWhiteList(text))
                    {
                        return true;
                    }
                    
                }
                */

                if (targetBp.Contains("eyelid"))
                {
                    targetBp = targetBp.Replace("eyelid", "eye");

                    bodyPartRegex = string.Format("( in the {0}[ |,])|({1}'s {0}) ", targetBp, defenderName);
                    //bodyPartRegex = string.Format(" in the {0}[ |,]", targetBp);
                    if (Regex.IsMatch(text, bodyPartRegex))
                    {
                        if (layerMatch || IsWhiteList(text) || singleLayerDent)
                        {
                            return true;
                        }
                    }
                }

                if (context.IsBiting(attackerName, defenderName))
                {
                    if (text.Contains(string.Format("{0} shakes {1} around", attackerName, defenderName)))
                    {
                        if (layerMatch || IsWhiteList(text) || singleLayerDent)
                        {
                            return true;
                        }
                        //return true;
                    }
                }
            }

            return false;
        }

        private static string[] WhiteList = new string[]{
            "and the injured part collapses into a lump of gore!",
            "and the injured part explodes into gore!",
            "and the injured part is ripped into loose shreds!",
            "and the severed part sails off in an arc!",
            "and the injured part collapses!",
            "and the injured part is cloven asunder!",
            "and the injured part is smashed into the body",
            "and the injured part is torn apart"
        };

        private static string[] NonCombatPatterns = new string[]{
            "stands up.",
            "struggles in vain",
            "but the attack is deflected",
            "grabs",
            "charges at",
            "collides with",
            "is knocked over",
            "adjusts the grip",
            "jumps away",
            "glances away",
            "down by the",
            "releases the grip"
             
        };
        private bool IsCombatText(string text)
        {
            return !NonCombatPatterns.Any(p => text.Contains(p));
                //&& (text.Contains(',') || IsWhiteList(text));
        }

        private bool IsWhiteList(string text)
        {
            return WhiteList.Any(f => text.Contains(f));
        }

        private void HandleAttackStart(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            context.Strike = new AttackStrike();
            context.Session.Strikes.Add(context.Strike);

            bool done = false;
            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.AttackEnd:
                        done = true;
                        break;
                    case SnifferTags.BodyPartAttackStart:
                        HandleBodyPartAttack(context, line, enumerator);
                        break;
                    case SnifferTags.DefenderWoundStart:
                        HandleDefenderWoundStart(context, line, enumerator);
                        break;
                    case SnifferTags.WeaponStart:
                        HandleWeapon(context, line, enumerator);
                        break;
                    case SnifferTags.ArmorStart:
                        HandleArmor(context, line, enumerator);
                        break;
                    default:
                        HandleKeyValueLine(context.Strike, line);
                        break;
                }
            }
        }

        private void HandleArmor(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var armor = new Armor();
            context.Strike.Armors.Add(armor);
            bool done = false;
            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.ArmorEnd:
                        done = true;
                        break;
                    default:
                        HandleKeyValueLine(armor, line);
                        break;
                }
            }
        }

        private void HandleWeapon(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            context.Weapon = new Weapon();
            context.Strike.Weapons.Add(context.Weapon);

            bool done = false;
            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.WeaponEnd:
                        done = true;
                        break;
                    case SnifferTags.WeaponAttackStart:
                        HandleWeaponAttack(context, line, enumerator);
                        break;
                    default:
                        HandleKeyValueLine(context.Weapon, line);
                        break;
                }
            }
        }

        private void HandleWeaponAttack(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var wa = new WeaponAttack();
            context.Weapon.Attacks.Add(wa);

            bool done = false;
            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.WeaponAttackEnd:
                        done = true;
                        break;
                    default:
                        HandleKeyValueLine(context.Weapon, line);
                        break;
                }
            }
        }

        private void HandleBodyPartAttack(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var bpAttack = new BodyPartAttack();
            context.Strike.BodyPartAttacks.Add(bpAttack);
            bool done = false;
            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.BodyPartAttackEnd:
                        done = true;
                        break;
                    default:
                        HandleKeyValueLine(bpAttack, line);
                        break;
                }
            }
        }

        private void HandleDefenderWoundStart(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var wound = new Wound();
            context.Wound = wound;
            context.Strike.Wounds.Add(wound);

            bool done = false;

            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.DefenderWoundEnd:
                        done = true;
                        break;
                    case SnifferTags.WoundBodyPartStart:
                        HandleWoundBodyPartStart(context, line, enumerator);
                        break;
                    default:
                        HandleKeyValueLine(wound, line);
                        break;
                }
            }
        }

        private void HandleTissueLayer(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var tl = new WoundBodyPartTissueLayer();
            context.WoundBodyPart.Layers.Add(tl);
            
            bool done = false;

            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.TissueLayerEnd:
                        done = true;
                        break;
                    default:
                        HandleKeyValueLine(tl, line);
                        break;
                }
            }
        }

        private void HandleWoundBodyPartStart(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var wbp = new WoundBodyPart();
            context.WoundBodyPart = wbp;
            context.Wound.Parts.Add(wbp);
            bool done = false;

            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.WoundBodyPartEnd:
                        done = true;
                        break;
                    case SnifferTags.TissueLayerStart:
                        HandleTissueLayer(context, line, enumerator);
                        break;
                    case SnifferTags.NoTissueLayerDefined:
                        break;
                    default:
                        HandleKeyValueLine(wbp, line);
                        break;
                }
            }
        }

        void HandleKeyValueLine(SnifferNode node, string line)
        {
            if (IsKeyValue(line))
            {
                var key = ParseKey(line);
                var value = ParseValue(line);
                node.KeyValues[key] = value.Trim();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private char[] KeyValueSeparator = new char[]{':'};
        private bool IsKeyValue(string key, string line)
        {
            if (!IsKeyValue(line)) return false;

            var lineKey = ParseKey(line);
            return lineKey.Equals(key);
        }

        private bool IsKeyValue(string line)
        {
            return line.Count(c => c == KeyValueSeparator[0]) > 0;
        }

        private string ParseKey(string keyValue)
        {
            return keyValue.Split(KeyValueSeparator, 2).First();
        }
        private string ParseValue(string keyValue)
        {
            return keyValue.Split(KeyValueSeparator, 2).Last();
        }

        private void HandleReportText(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            // TODO - perhaps here is the best place to handle the strike text

            var v = ParseValue(line);
            context.Session.ReportTexts.Add(v.Trim());
        }

        private void HandleSessionStart(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            context.Session = new SnifferSession();
            context.Strike = null;
            context.Wound = null;
            context.Weapon = null;
            context.WoundBodyPart = null;
            context.Data.Sessions.Add(context.Session);
        }
    }
}