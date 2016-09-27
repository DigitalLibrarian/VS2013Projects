using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfCombatSnifferReaderApp
{
    public static class SnifferTags
    {
        public const string SessionStart = "COMBAT_SNIFFER_SESSION_START";
        public const string ReportText = "REPORT_TEXT";
        public const string AttackStart = "UNIT_ATTACK_START";
        public const string AttackEnd = "UNIT_ATTACK_END";
        public const string BodyPartAttackStart = "START_BODY_PART_ATTACK";
        public const string BodyPartAttackEnd = "END_BODY_PART_ATTACK";
        public const string TissueLayerStart = "TISSUE_LAYER_START";
        public const string TissueLayerEnd = "TISSUE_LAYER_END";
        public const string DefenderWoundStart = "DEFENDER_WOUND_START";
        public const string DefenderWoundEnd = "DEFENDER_WOUND_END";
        public const string WoundBodyPartStart = "WOUND_BODY_PART_START";
        public const string WoundBodyPartEnd = "WOUND_BODY_PART_END";

        public const string ArmorStart = "START_ARMOR";
        public const string ArmorEnd = "END_ARMOR";
        public const string WeaponStart = "START_WEAPON";
        public const string WeaponEnd = "END_WEAPON";
        public const string WeaponAttackStart = "START_WEAPON_ATTACK";
        public const string WeaponAttackEnd = "END_WEAPON_ATTACK";
        public const string NoTissueLayerDefined = "NO_TISSUE_LAYER_DEFINED";

        public const string AttackerName = "ATTACKER";
        public const string DefenderName = "DEFENDER";
        public const string BodyPartNameSingular = "BODY_PART_NAME_SINGULAR";
        public const string BodyPartNamePlural = "BODY_PART_NAME_PLURAL";
        public const string BodyPartId = "BODY_PART_ID";

        public const string Material = "MATERIAL";
        public const string TissueLayerName = "LAYER";
        public const string Severed = "SEVERED_PART";

        public const string WoundId = "Wound ID";
        public const string CutFraction = "CUT_FRACTION";
        public const string DentFraction = "DENT_FRACTION";
        public const string EffectFraction = "EFFECT_FRACTION";

        public const string UnitStart = "UNIT_START";
        public const string UnitEnd = "UNIT_END";
        public const string BodyStart = "BODY_START";
        public const string BodyEnd = "BODY_END";
        public const string BodyPartStart = "BODY_PART_START";
        public const string BodyPartEnd = "BODY_PART_END";

        public const string NameSingular = "NAME_SINGULAR";
        public const string Name = "NAME";

        public const string MaterialName = "MATERIAL_NAME";
        public const string ItemSubTypeName = "ITEM_SUB_TYPE_NAME";

        public const string Id = "ID";
    }
}
