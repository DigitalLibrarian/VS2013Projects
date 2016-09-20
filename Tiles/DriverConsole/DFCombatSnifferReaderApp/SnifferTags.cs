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
    }
}
