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
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> ReportTexts { get; set; }
        public List<AttackStrike> Strikes { get; set; }
        public List<Unit> Units { get; set; }
        public SnifferSession(int id)
        {
            Id = id;
            ReportTexts = new List<string>();
            Strikes = new List<AttackStrike>();
            Units = new List<Unit>();
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

    public class Unit : SnifferNode
    {
        public UnitBody Body { get; set; }
        public List<BodyPartAttack> BodyPartAttacks { get; set; }
        public List<Weapon> Weapons { get; set; }
        public List<Armor> Armors { get; set; }

        public Unit()
        {
            BodyPartAttacks = new List<BodyPartAttack>();
            Weapons = new List<Weapon>();
            Armors = new List<Armor>();
        }

        public string Name { get { return KeyValues[SnifferTags.Name]; } }

        public int Id { get { return int.Parse(KeyValues[SnifferTags.Id]); } }
    }

    public class UnitBody : SnifferNode
    {
        public List<UnitBodyPart> BodyParts { get; set; }
        public UnitBody()
        {
            BodyParts = new List<UnitBodyPart>();
        }
    }

    public class UnitBodyPart : SnifferNode
    {
        public List<TissueLayer> Layers { get; set; }
        public UnitBodyPart()
        {
            Layers = new List<TissueLayer>();
        }

        public string Name { get { return KeyValues[SnifferTags.NameSingular]; } }
    }

    public class BodyPartAttack : SnifferNode
    {
        public string Name { get { return KeyValues[SnifferTags.Name]; } }
    }

    public class Armor : SnifferNode
    {

        public string Name { 
            get
            {
                var materialName = KeyValues.ContainsKey(SnifferTags.MaterialName) ? KeyValues[SnifferTags.MaterialName] : "<unknown material>";
                var typeName = KeyValues.ContainsKey(SnifferTags.ItemSubTypeName) ? KeyValues[SnifferTags.ItemSubTypeName] : "<unknown weapon type>";

                return string.Format("{0} {1}", materialName, typeName);
            }
        }
    }

    public class AttackStrike : SnifferNode
    {
        public List<Wound> Wounds { get; private set; }
        public int ReportTextIndex { get; set; }
        public AttackStrike()
            : base()
        {
            ReportTextIndex = -1;
            Wounds = new List<Wound>();
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
        public List<TissueLayer> Layers { get; private set; }

        public WoundBodyPart()
            : base()
        {
            Layers = new List<TissueLayer>();
        }
    }

    public class TissueLayer : SnifferNode
    {
        public string Name { get { return KeyValues[SnifferTags.TissueLayerName]; } }
    }

    public class Weapon : SnifferNode
    {
        public List<WeaponAttack> Attacks { get; private set; }
        public Weapon()
            : base()
        {
            Attacks = new List<WeaponAttack>();
        }

        /*
         * MATERIAL_NAME: wood
ITEM_SUB_TYPE_NAME: short swor
         * */
        public string Name
        {
            get
            {
                var materialName = KeyValues.ContainsKey(SnifferTags.MaterialName) ? KeyValues[SnifferTags.MaterialName] : "<unknown material>";
                var typeName = KeyValues.ContainsKey(SnifferTags.ItemSubTypeName) ? KeyValues[SnifferTags.ItemSubTypeName] : "<unknown weapon type>";

                return string.Format("{0} {1}", materialName, typeName);
            }
        }
    }

    public class WeaponAttack : SnifferNode
    {

        public string Name { get { return KeyValues[SnifferTags.Name]; } }
    }

    public interface ISnifferLogParser
    {
        ISnifferLogData Parse(IEnumerable<string> lines);
        bool IsCombatText(string text);
    }
}
