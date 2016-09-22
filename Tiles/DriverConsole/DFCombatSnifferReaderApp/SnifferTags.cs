using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfCombatSnifferReaderApp
{
    public static class SnifferTags
    {
        public const string SessionStart = "COMBAT SNIFFER SESSION START";
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
        public const string BodyPartName = "BODY_PART_NAME";
    }
}
