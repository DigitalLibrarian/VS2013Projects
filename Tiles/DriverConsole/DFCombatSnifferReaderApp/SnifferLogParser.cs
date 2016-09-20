using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfCombatSnifferReaderApp
{
    public interface ISnifferLogData
    {
        List<SnifferSession> Sessions { get; }
    }

    public class SnifferLogData : ISnifferLogData
    {
        public List<SnifferSession> Sessions { get; private set; }
        public SnifferLogData()
        {
            Sessions = new List<SnifferSession>();
        }
    }

    public class SnifferSession
    {
        public List<string> ReportTexts { get; set; }
        public List<AttackStrike> Strikes { get; set; }
        public SnifferSession()
        {
            ReportTexts = new List<string>();
            Strikes = new List<AttackStrike>();
        }
    }

    public class SnifferNode
    {
        public Dictionary<string, string> KeyValues { get; set; }
        public SnifferNode()
        {
            KeyValues = new Dictionary<string, string>();
        }
    }

    public class BodyPartAttack : SnifferNode
    {

    }

    public class AttackStrike : SnifferNode
    {
        public List<BodyPartAttack> BodyPartAttacks { get; private set; }
        public List<Wound> Wounds { get; private set; }
        public List<Weapon> Weapons { get; private set; }
        public string ReportText { get; set; }
        public AttackStrike() : base()
        {
            BodyPartAttacks = new List<BodyPartAttack>();
            Wounds = new List<Wound>();
            Weapons = new List<Weapon>();
        }
    }

    public class Wound : SnifferNode
    {
        public List<WoundBodyPart> Parts { get; private set; }
        public Wound() : base()
        {
            Parts = new List<WoundBodyPart>();
        }
    }

    public class WoundBodyPart : SnifferNode
    {
        public List<WoundBodyPartTissueLayer> Layers { get; private set; }

        public WoundBodyPart() : base()
        {
            Layers = new List<WoundBodyPartTissueLayer>();
        }
    }

    public class WoundBodyPartTissueLayer : SnifferNode
    {

    }

    public class Weapon : SnifferNode
    {
        public List<WeaponAttack> Attacks { get; private set; }
        public Weapon() : base()
        {
            Attacks = new List<WeaponAttack>();
        }
    }

    public class WeaponAttack : SnifferNode
    {

    }

    public interface ISnifferLogParser
    {
        ISnifferLogData Parse(IEnumerable<string> lines);
    }


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
        }
    }

    public class SnifferLogParser : ISnifferLogParser
    {
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

            return context.Data;
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
                        // TODO - this does not work.  The correct line is always
                        // adjacent to our position here, but it might be behind as well as in-front.
                        // We really need a set of hueristic rules for determining how close a 
                        // log line is to the start of parsing at this point
                        while (!IsKeyValue(SnifferTags.ReportText, line))
                        {
                            enumerator.MoveNext();
                            line = enumerator.Current;
                        }
                        context.Strike.ReportText = ParseValue(line);
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
                    default:
                        HandleKeyValueLine(context.Strike, line);
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
                    case SnifferTags.TissueLayerStart:
                        HandleTissueLayer(context, line, enumerator);
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
                node.KeyValues[key] = value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private const char KeyValueSeparator = ':';
        private bool IsKeyValue(string key, string line)
        {
            if (!IsKeyValue(line)) return false;

            var lineKey = ParseKey(line);
            return lineKey.Equals(key);
        }

        private bool IsKeyValue(string line)
        {
            return line.Contains(KeyValueSeparator);
        }

        private string ParseKey(string keyValue)
        {
            return keyValue.Split(KeyValueSeparator).First();
        }
        private string ParseValue(string keyValue)
        {
            return keyValue.Split(KeyValueSeparator).Skip(1).First();
        }

        private void HandleReportText(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var v = ParseValue(line);
            context.Session.ReportTexts.Add(v);
        }

        private void HandleSessionStart(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            context.Session = new SnifferSession();
            context.Data.Sessions.Add(context.Session);
        }
    }
}
