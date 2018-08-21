using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws
{
    public static class DfTags
    {
        public const string OBJECT = "OBJECT";
        public const string CREATURE = "CREATURE";
        public const string CREATURE_VARIATION = "CREATURE_VARIATION";
        public const string BODY = "BODY";
        public const string BODY_DETAIL_PLAN = "BODY_DETAIL_PLAN";
        public const string MATERIAL_TEMPLATE = "MATERIAL_TEMPLATE";
        public const string TISSUE_TEMPLATE = "TISSUE_TEMPLATE";
        public const string INORGANIC = "INORGANIC";
        public const string ITEM_WEAPON = "ITEM_WEAPON";
        public const string ITEM_SHOES = "ITEM_SHOES";
        public const string ITEM_ARMOR = "ITEM_ARMOR";
        public const string ITEM_PANTS = "ITEM_PANTS";
        public const string ITEM_GLOVES = "ITEM_GLOVES";
        public const string ITEM_HELM = "ITEM_HELM";
        public const string ITEM_TOOL = "ITEM_TOOL";

        public static string[] GetAllObjectTypes()
        {
            return new string[]{
                CREATURE,
                CREATURE_VARIATION,
                BODY,
                BODY_DETAIL_PLAN,
                MATERIAL_TEMPLATE,
                TISSUE_TEMPLATE,
                INORGANIC,
                ITEM_WEAPON,
                ITEM_SHOES,
                ITEM_ARMOR,
                ITEM_SHOES,
                ITEM_PANTS,
                ITEM_GLOVES,
                ITEM_HELM,
                ITEM_TOOL
            };
        }

        public static string[] GetAllWeaponTypes()
        {
            return new string[]{
                DfTags.MiscTags.ITEM_WEAPON_WHIP ,
                DfTags.MiscTags.ITEM_WEAPON_AXE_BATTLE ,
                DfTags.MiscTags.ITEM_WEAPON_HAMMER_WAR ,
                DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT ,
                DfTags.MiscTags.ITEM_WEAPON_SPEAR ,
                DfTags.MiscTags.ITEM_WEAPON_MACE ,
                DfTags.MiscTags.ITEM_WEAPON_CROSSBOW ,
                DfTags.MiscTags.ITEM_WEAPON_PICK ,
                DfTags.MiscTags.ITEM_WEAPON_BOW ,
                DfTags.MiscTags.ITEM_WEAPON_BLOWGUN ,
                DfTags.MiscTags.ITEM_WEAPON_PIKE ,
                DfTags.MiscTags.ITEM_WEAPON_HALBERD ,
                DfTags.MiscTags.ITEM_WEAPON_SWORD_2H ,
                DfTags.MiscTags.ITEM_WEAPON_SWORD_LONG ,
                DfTags.MiscTags.ITEM_WEAPON_MAUL ,
                DfTags.MiscTags.ITEM_WEAPON_AXE_GREAT ,
                DfTags.MiscTags.ITEM_WEAPON_DAGGER_LARGE ,
                DfTags.MiscTags.ITEM_WEAPON_SCOURGE ,
                DfTags.MiscTags.ITEM_WEAPON_FLAIL ,
                DfTags.MiscTags.ITEM_WEAPON_MORNINGSTAR ,
                DfTags.MiscTags.ITEM_WEAPON_SCIMITAR ,
                DfTags.MiscTags.ITEM_WEAPON_AXE_TRAINING ,
                DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT_TRAINING ,
                DfTags.MiscTags.ITEM_WEAPON_SPEAR_TRAINING
            };
        }
        
        public static class MiscTags
        {
            public const string CV_REMOVE_TAG = "CV_REMOVE_TAG";
            public const string CV_NEW_TAG = "CV_NEW_TAG";
            public const string CV_CONVERT_TAG = "CV_CONVERT_TAG";
            public const string CVCT_MASTER = "CVCT_MASTER";
            public const string CVCT_TARGET = "CVCT_TARGET";
            public const string CVCT_REPLACEMENT = "CVCT_REPLACEMENT";

            public const string COPY_TAGS_FROM = "COPY_TAGS_FROM";
            public const string GO_TO_START = "GO_TO_START";
            public const string GO_TO_END = "GO_TO_END";
            public const string GO_TO_TAG = "GO_TO_TAG";

            public const string APPLY_CREATURE_VARIATION = "APPLY_CREATURE_VARIATION";
            public const string CASTE = "CASTE";
            public const string SELECT_CASTE = "SELECT_CASTE";
            public const string SELECT_ADDITIONAL_CASTE = "SELECT_ADDITIONAL_CASTE";
            public const string MALE = "MALE";
            public const string FEMALE = "FEMALE";

            public const string BP = "BP";
            public const string BP_LAYERS = "BP_LAYERS";
            public const string BP_RELSIZE = "BP_RELSIZE";
            public const string RELSIZE = "RELSIZE";
            public const string BP_RELATION = "BP_RELATION";
            public const string BP_POSITION = "BP_POSITION";
            public const string BY_CATEGORY = "BY_CATEGORY";
            public const string BY_TYPE= "BY_TYPE";

            public const string USE_MATERIAL_TEMPLATE = "USE_MATERIAL_TEMPLATE";
            public const string ADD_MATERIAL = "ADD_MATERIAL";
            public const string START_MATERIAL = "DfNet.START_MATERIAL";
            public const string END_MATERIAL = "DfNet.END_MATERIAL";
            public const string REMOVE_MATERIAL = "REMOVE_MATERIAL";

            public const string TISSUE = "TISSUE";
            public const string START_TISSUE = "DfNet.START_TISSUE";
            public const string END_TISSUE = "DfNet.END_TISSUE";
            public const string ADD_TISSUE = "ADD_TISSUE";
            public const string REMOVE_TISSUE = "REMOVE_TISSUE";
            public const string USE_TISSUE_TEMPLATE = "USE_TISSUE_TEMPLATE";
            public const string TISSUE_LAYER = "TISSUE_LAYER";
            public const string SELECT_TISSUE_LAYER = "SELECT_TISSUE_LAYER";
            public const string PLUS_TISSUE_LAYER = "PLUS_TISSUE_LAYER";
            public const string TISSUE_NAME = "TISSUE_NAME";
            public const string TL_MAJOR_ARTERIES = "TL_MAJOR_ARTERIES";
            public const string SET_LAYER_TISSUE = "SET_LAYER_TISSUE";

            public const string BLOOD = "BLOOD";
            public const string PUS = "PUS";

            public const string NOPAIN = "NOPAIN";

            public const string VASCULAR = "VASCULAR";
            public const string PAIN_RECEPTORS = "PAIN_RECEPTORS";
            public const string HEALING_RATE = "HEALING_RATE";
            public const string CONNECTS = "CONNECTS";
            public const string COSMETIC = "COSMETIC";
            public const string ARTERIES = "ARTERIES";
            public const string RELATIVE_THICKNESS = "RELATIVE_THICKNESS";
            public const string THICKENS_ON_STRENGTH = "THICKENS_ON_STRENGTH";
            public const string THICKENS_ON_ENERGY_STORAGE = "THICKENS_ON_ENERGY_STORAGE";

            public const string TISSUE_MATERIAL = "TISSUE_MATERIAL";

            public const string CASTE_NAME = "CASTE_NAME";
            public const string ATTACK = "ATTACK";
            public const string ATTACK_VERB = "ATTACK_VERB";
            public const string ATTACK_CONTACT_PERC = "ATTACK_CONTACT_PERC";
            public const string ATTACK_PENETRATION_PERC = "ATTACK_PENETRATION_PERC";
            public const string ATTACK_PREPARE_AND_RECOVER = "ATTACK_PREPARE_AND_RECOVER";
            public const string ATTACK_FLAG_EDGE = "ATTACK_FLAG_EDGE";

            public const string CATEGORY = "CATEGORY";

            public const string CON = "CON";
            public const string CON_CAT = "CON_CAT";
            public const string CONTYPE = "CONTYPE";

            public const string STATE_NAME = "STATE_NAME";
            public const string STATE_NAME_ADJ = "STATE_NAME_ADJ";
            public const string STATE_ADJ = "STATE_ADJ";
            public const string ALL_SOLID = "ALL_SOLID";
            public const string ALL = "ALL";
            public const string SOLID = "SOLID";

            public const string SOLID_DENSITY = "SOLID_DENSITY";

            public const string GRASP = "GRASP";
            public const string STANCE = "STANCE";

            public const string HEAD = "HEAD";
            public const string LIMB = "LIMB";
            public const string DIGIT = "DIGIT";

            public const string NAME = "NAME";
            public const string LAYER = "LAYER";

            public const string IS_METAL = "IS_METAL";
            public const string IS_GEM = "IS_GEM";
            public const string NONE = "NONE";

            public const string TILE = "TILE";
            public const string SIZE = "SIZE";
            public const string LAYER_SIZE = "LAYER_SIZE";
            public const string CREATURE_TILE = "CREATURE_TILE";

            public const string COLOR = "COLOR";

            public const string PHYS_ATT_RANGE = "PHYS_ATT_RANGE";
            public const string MENT_ATT_RANGE = "MENT_ATT_RANGE";

            public const string IMPACT_YIELD = "IMPACT_YIELD";
            public const string IMPACT_FRACTURE = "IMPACT_FRACTURE";
            public const string IMPACT_STRAIN_AT_YIELD = "IMPACT_STRAIN_AT_YIELD";

            public const string SHEAR_YIELD = "SHEAR_YIELD";
            public const string SHEAR_FRACTURE = "SHEAR_FRACTURE";
            public const string SHEAR_STRAIN_AT_YIELD = "SHEAR_STRAIN_AT_YIELD";

            public const string COMPRESSIVE_YIELD = "COMPRESSIVE_YIELD";
            public const string COMPRESSIVE_FRACTURE = "COMPRESSIVE_FRACTURE";
            public const string COMPRESSIVE_STRAIN_AT_YIELD = "COMPRESSIVE_STRAIN_AT_YIELD";

            public const string TENSILE_YIELD = "TENSILE_YIELD";
            public const string TENSILE_FRACTURE = "TENSILE_FRACTURE";
            public const string TENSILE_STRAIN_AT_YIELD = "TENSILE_STRAIN_AT_YIELD";

            public const string TORSION_YIELD = "TORSION_YIELD";
            public const string TORSION_FRACTURE = "TORSION_FRACTURE";
            public const string TORSION_STRAIN_AT_YIELD = "TORSION_STRAIN_AT_YIELD";

            public const string BENDING_YIELD = "BENDING_YIELD";
            public const string BENDING_FRACTURE = "BENDING_FRACTURE";
            public const string BENDING_STRAIN_AT_YIELD = "BENDING_STRAIN_AT_YIELD";

            public const string MAX_EDGE = "MAX_EDGE";

            public const string DEFAULT_RELSIZE = "DEFAULT_RELSIZE";
            public const string BODY_SIZE = "BODY_SIZE";

            public const string ITEM_WEAPON_WHIP  = "ITEM_WEAPON_WHIP";
            public const string ITEM_WEAPON_AXE_BATTLE  = "ITEM_WEAPON_AXE_BATTLE";
            public const string ITEM_WEAPON_HAMMER_WAR  = "ITEM_WEAPON_HAMMER_WAR";
            public const string ITEM_WEAPON_SWORD_SHORT  = "ITEM_WEAPON_SWORD_SHORT";
            public const string ITEM_WEAPON_SPEAR  = "ITEM_WEAPON_SPEAR";
            public const string ITEM_WEAPON_MACE  = "ITEM_WEAPON_MACE";
            public const string ITEM_WEAPON_CROSSBOW  = "ITEM_WEAPON_CROSSBOW";
            public const string ITEM_WEAPON_PICK  = "ITEM_WEAPON_PICK";
            public const string ITEM_WEAPON_BOW  = "ITEM_WEAPON_BOW";
            public const string ITEM_WEAPON_BLOWGUN  = "ITEM_WEAPON_BLOWGUN";
            public const string ITEM_WEAPON_PIKE  = "ITEM_WEAPON_PIKE";
            public const string ITEM_WEAPON_HALBERD  = "ITEM_WEAPON_HALBERD";
            public const string ITEM_WEAPON_SWORD_2H  = "ITEM_WEAPON_SWORD_2H";
            public const string ITEM_WEAPON_SWORD_LONG  = "ITEM_WEAPON_SWORD_LONG";
            public const string ITEM_WEAPON_MAUL  = "ITEM_WEAPON_MAUL";
            public const string ITEM_WEAPON_AXE_GREAT  = "ITEM_WEAPON_AXE_GREAT";
            public const string ITEM_WEAPON_DAGGER_LARGE  = "ITEM_WEAPON_DAGGER_LARGE";
            public const string ITEM_WEAPON_SCOURGE  = "ITEM_WEAPON_SCOURGE";
            public const string ITEM_WEAPON_FLAIL  = "ITEM_WEAPON_FLAIL";
            public const string ITEM_WEAPON_MORNINGSTAR  = "ITEM_WEAPON_MORNINGSTAR";
            public const string ITEM_WEAPON_SCIMITAR  = "ITEM_WEAPON_SCIMITAR";
            public const string ITEM_WEAPON_AXE_TRAINING  = "ITEM_WEAPON_AXE_TRAINING";
            public const string ITEM_WEAPON_SWORD_SHORT_TRAINING  = "ITEM_WEAPON_SWORD_SHORT_TRAINING";
            public const string ITEM_WEAPON_SPEAR_TRAINING  = "ITEM_WEAPON_SPEAR_TRAINING";
        }

    }
}
