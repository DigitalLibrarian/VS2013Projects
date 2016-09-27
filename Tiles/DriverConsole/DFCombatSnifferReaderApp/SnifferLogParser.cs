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

            public Unit Unit { get; set; }
            public UnitBodyPart UnitBodyPart { get; set; }
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

            internal int GetNewSessionId()
            {
                return Data.Sessions.Count() + 1;
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
                        else if (IsKeyValue(SnifferTags.SessionStart, line))
                        {
                            HandleSessionStart(context, line, enumerator);
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
            var used = new Dictionary<int, bool>();
            var reportTexts = session.ReportTexts.ToList();

            foreach(var strike in session.Strikes)
            {
                int index = 0;
                while (strike.ReportTextIndex == -1 && index < reportTexts.Count())
                {
                    if (!used.ContainsKey(index))
                    {
                        var reportText = reportTexts[index];
                        if (IsMatch(context, strike, reportText))
                        {
                            strike.ReportTextIndex = index;
                            used[index] = true;
                            HandleBiting(context, session, strike);
                            break;
                        }
                    }

                    index++;
                }
            }
        }
        
        private void HandleBiting(ParserContext context, SnifferSession session, AttackStrike strike)
        {
            var text = session.GetReportText(strike);
            if (text == null) return;
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

        TissueLayer PickLayer(AttackStrike strike)
        {
            var part = strike.Wounds.Last().Parts.LastOrDefault(p =>
                {
                    int dent = p.KeyValues.ContainsKey(SnifferTags.DentFraction) ? int.Parse(p.KeyValues[SnifferTags.DentFraction]) : 0;
                    int cut = p.KeyValues.ContainsKey(SnifferTags.CutFraction) ? int.Parse(p.KeyValues[SnifferTags.CutFraction]) : 0;
                    int effect = p.KeyValues.ContainsKey(SnifferTags.EffectFraction) ? int.Parse(p.KeyValues[SnifferTags.EffectFraction]) : 0;
                    return dent > 0 || cut > 0 || effect > 0;
                });
            if (part == null) return null;

            if (part.Layers.Any())
            {
                return part.Layers.Last();
            }
            else
            {
                return null;
            }
        }

        private bool IsMatch(ParserContext context, AttackStrike strike, string text)
        {
            if (!IsCombatText(text)) return false;

            var attackerName = strike.KeyValues[SnifferTags.AttackerName];
            var defenderName = strike.KeyValues[SnifferTags.DefenderName];

            if (!strike.Wounds.Any())
            {
                return false;
                /*
                return text.StartsWith(attackerName)
                    && text.Contains(defenderName)
                    && IsWhiteList(text);
                 * */
            }

            var wound = strike.Wounds.Last();
            var severedWound = wound.KeyValues[SnifferTags.Severed].ToLower() == "true";
            var severedText = text.Contains("severed");

            if (severedText != severedWound) return false;

            var targetBp = strike.Wounds.Last().Parts.First().KeyValues[SnifferTags.BodyPartNameSingular];
            var targetBpPlural = strike.Wounds.Last().Parts.First().KeyValues[SnifferTags.BodyPartNamePlural];
            var layerName = "NERP";
            var material = "NERP";

            var lastLayer = PickLayer(strike);
            if(lastLayer != null)
            {
                layerName = lastLayer.KeyValues[SnifferTags.TissueLayerName].ToLower();
                material = lastLayer.KeyValues[SnifferTags.Material].ToLower();
            }

            var lastBpName = strike.Wounds.Last().Parts.Last().KeyValues[SnifferTags.BodyPartNameSingular];
            var lastBpPlural = strike.Wounds.Last().Parts.Last().KeyValues[SnifferTags.BodyPartNamePlural];
            

            var combatantRegex = string.Format("^{0} .+? {1}['| ]", attackerName, defenderName);
            bool isCombatText = IsCombatText(text);
            bool isCombatantRegex = Regex.IsMatch(text, combatantRegex);
            bool hasBut = Regex.IsMatch(text, ", but ");
            var result = isCombatText && isCombatantRegex && !hasBut;
            
            var layerRegex = string.Format("(({1}'s )|())(({0})|({1})|({2})|({3}))( collapses)*", layerName, lastBpName, lastBpPlural, material);
            bool singleLayerDent = Regex.IsMatch(text, "((tear)|(bruis)|(shatter)|(dent)|(fractur))ing it!");
            bool lastBpCollapse = Regex.IsMatch(text, string.Format("{0} collapses", lastBpName));
            bool layerMatch = Regex.IsMatch(text, layerRegex);

            if (!isCombatText || hasBut) return false;

            if (isCombatantRegex)
            {
                if (lastBpCollapse) return true;
                
                var bodyPartRegex = string.Format("( in the (({0})|({1}))[ |,|!])|({2}'s {0})", targetBp, targetBpPlural, defenderName);

                if (Regex.IsMatch(text, bodyPartRegex))
                {
                    if (layerMatch || IsWhiteList(text) || singleLayerDent || text.Contains( " bites "))
                    {
                        return true;
                    }   
                }

                if (targetBp.Contains("eyelid"))
                {
                    targetBp = targetBp.Replace("eyelid", "eye");

                    bodyPartRegex = string.Format("( in the {0}[ |,])|({1}'s {0}) ", targetBp, defenderName);
                    if (Regex.IsMatch(text, bodyPartRegex))
                    {
                        if (layerMatch || IsWhiteList(text) || singleLayerDent)
                        {
                            return true;
                        }
                    }
                }

                if (text.Contains(string.Format("{0} shakes {1} around by the {2}", attackerName, defenderName, targetBp)))
                {
                    if (layerMatch || IsWhiteList(text) || singleLayerDent)
                    {
                        return true;
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
            "and the injured part is torn apart",
            "and the injured part is crushed",
            "and the part splits in gore"
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
            "releases the grip",
            "places a chokehold",
            "breaks the grip",
            "break the grip",
            "latches on firmly",
            " locks ",
            "An artery has been opened",
            " ligament ",
            " tendon ",
            " artery ",
            " nerve",
            " misses ", 
            ": ",
            "unconscious",
            "stunned",
            "enraged",
            " bounces ",
            " falls ",
            " looks sick",
            " looks even more sick",
            " vomits",
            " retches",
            " is ripped away",
            "conscious",
            "drop away",
            "passes out",
            "having trouble breathing",
            "releases",
            " but ",
            "lodged firmly",
            "pulls on the embedded",
            "loses hold",
            "has been struck down",
            "gives in to pain",
            "fall over",
            "skids along",
            "flight path",
            "slams into an obstacle",
            "has bled to death",
            "propelled away",
            "is interrupted",
            "sucked out of the wound",
            "is injected"
        };
        public bool IsCombatText(string text)
        {
            if(NonCombatPatterns.Any(p => text.Contains(p))) return false;
            //if (IsWhiteList(text)) return true;
            return true;
        }

        private bool IsWhiteList(string text)
        {
            return WhiteList.Any(f => text.Contains(f));
        }

        private void HandleAttackStart(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            context.Strike = new AttackStrike();

            bool done = false;
            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.AttackEnd:
                        done = true;
                        break;
                    case SnifferTags.DefenderWoundStart:
                        HandleDefenderWoundStart(context, line, enumerator);
                        break;
                    case SnifferTags.UnitStart:
                        HandleUnit(context, line, enumerator);
                        break;
                    default:
                        HandleKeyValueLine(context.Strike, line);
                        break;
                }
            }

            var strikeWoundId = context.Strike.KeyValues[SnifferTags.WoundId];
            if (strikeWoundId == "-1" || !context.Session.Strikes.Any(strike => 
                context.Strike.KeyValues[SnifferTags.DefenderName].Equals(strike.KeyValues[SnifferTags.DefenderName])
                 && strike.KeyValues[SnifferTags.WoundId].Equals(strikeWoundId)))
            {
                context.Session.Strikes.Add(context.Strike);
            }
        }

        private void HandleUnit(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var unit = new Unit();
            context.Unit = unit;
            context.Session.Units.Add(unit);

            bool done = false;
            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.UnitEnd:
                        done = true;
                        break;

                    case SnifferTags.BodyPartAttackStart:
                        HandleBodyPartAttack(context, line, enumerator);
                        break;
                    case SnifferTags.WeaponStart:
                        HandleWeapon(context, line, enumerator);
                        break;
                    case SnifferTags.ArmorStart:
                        HandleArmor(context, line, enumerator);
                        break;

                    case SnifferTags.BodyStart:
                        HandleBody(context, line, enumerator);
                        break;
                    default:
                        HandleKeyValueLine(unit, line);
                        break;
                }
            }
        }

        private void HandleBody(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var body = new UnitBody();
            context.Unit.Body = body;
            bool done = false;
            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.BodyEnd:
                        done = true;
                        break;
                    case SnifferTags.BodyPartStart:
                        HandleBodyPart(context, line, enumerator);
                        break;
                    default:
                        HandleKeyValueLine(body, line);
                        break;
                }
            }
        }

        private void HandleBodyPart(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var bodyPart = new UnitBodyPart();

            context.UnitBodyPart = bodyPart;

            bool done = false;
            while (!done && enumerator.MoveNext())
            {
                line = enumerator.Current;

                switch (line)
                {
                    case SnifferTags.BodyPartEnd:
                        done = true;
                        break;
                    case SnifferTags.TissueLayerStart:
                        HandleTissueLayer(context, line, enumerator);
                        break;
                    default:
                        HandleKeyValueLine(bodyPart, line);
                        break;
                }
            }
        }

        private void HandleArmor(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var armor = new Armor();
            //context.Strike.Armors.Add(armor);
            context.Unit.Armors.Add(armor);
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
            //context.Strike.Weapons.Add(context.Weapon);
            context.Unit.Weapons.Add(context.Weapon);

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
            //context.Strike.BodyPartAttacks.Add(bpAttack);
            context.Unit.BodyPartAttacks.Add(bpAttack);
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
            var tl = new TissueLayer();
            //context.WoundBodyPart.Layers.Add(tl);
            context.UnitBodyPart.Layers.Add(tl);
            
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
            var sessionId = context.GetNewSessionId();
            context.Session = new SnifferSession(sessionId);
            context.Strike = null;
            context.Wound = null;
            context.Weapon = null;
            context.WoundBodyPart = null;
            
            if (IsKeyValue(SnifferTags.SessionStart, line))
            {
                context.Session.Name = ParseValue(line);
            }

            context.Data.Sessions.Add(context.Session);
        }
    }
}