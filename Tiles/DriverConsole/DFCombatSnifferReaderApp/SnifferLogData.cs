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

        public string GetReportText(AttackStrike strike)
        {
            if (strike.ReportTextIndex == -1)
            {
                return null;
            }
            return ReportTexts[strike.ReportTextIndex];
        }
    }

    public class SnifferNode
    {
        public Dictionary<string, string> KeyValues { get; private set; }
        public SnifferNode()
        {
            KeyValues = new Dictionary<string, string>();
        }
    }

    public class BodyPartAttack : SnifferNode
    {

    }

    public class Armor : SnifferNode
    {

    }

    public class AttackStrike : SnifferNode
    {
        public List<BodyPartAttack> BodyPartAttacks { get; private set; }
        public List<Wound> Wounds { get; private set; }
        public List<Weapon> Weapons { get; private set; }
        public List<Armor> Armors { get; private set; }
        public int ReportTextIndex { get; set; }
        public AttackStrike()
            : base()
        {
            BodyPartAttacks = new List<BodyPartAttack>();
            Wounds = new List<Wound>();
            Weapons = new List<Weapon>();
            Armors = new List<Armor>();
            ReportTextIndex = -1;
        }

        public string AttackerName { get { return KeyValues[SnifferTags.AttackerName]; } }
        public string DefenderName { get { return KeyValues[SnifferTags.DefenderName]; } }
    }

    public class Wound : SnifferNode
    {
        public List<WoundBodyPart> Parts { get; private set; }
        public Wound()
            : base()
        {
            Parts = new List<WoundBodyPart>();
        }
    }

    public class WoundBodyPart : SnifferNode
    {
        public List<WoundBodyPartTissueLayer> Layers { get; private set; }

        public WoundBodyPart()
            : base()
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
        public Weapon()
            : base()
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
        bool IsCombatText(string text);
    }
}
