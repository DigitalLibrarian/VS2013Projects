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
        public const string ITEM_WEAPON = "ITEM_WEAPON";
        public const string ITEM_SHOES = "ITEM_SHOES";
        public const string INORGANIC = "INORGANIC";

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
                ITEM_SHOES
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
            public const string BP_RELATION = "BP_RELATION";
            public const string BP_POSITION = "BP_POSITION";
            public const string BY_CATEGORY = "BY_CATEGORY";

            public const string USE_MATERIAL_TEMPLATE = "USE_MATERIAL_TEMPLATE";
            public const string ADD_MATERIAL = "ADD_MATERIAL";
            public const string START_MATERIAL = "DfNet.START_MATERIAL";
            public const string END_MATERIAL = "DfNet.END_MATERIAL";
            public const string REMOVE_MATERIAL = "REMOVE_MATERIAL";

            public const string START_TISSUE = "DfNet.START_TISSUE";
            public const string END_TISSUE = "DfNet.END_TISSUE";
            public const string ADD_TISSUE = "ADD_TISSUE";
            public const string REMOVE_TISSUE = "REMOVE_TISSUE";
            public const string USE_TISSUE_TEMPLATE = "USE_TISSUE_TEMPLATE";
            public const string TISSUE_LAYER = "TISSUE_LAYER";
            public const string SELECT_TISSUE_LAYER = "SELECT_TISSUE_LAYER";
            public const string PLUS_TISSUE_LAYER = "PLUS_TISSUE_LAYER";
            public const string TISSUE_NAME = "TISSUE_NAME";

            public const string CASTE_NAME = "CASTE_NAME";
            public const string ATTACK = "ATTACK";
            public const string ATTACK_PREPARE_AND_RECOVER = "ATTACK_PREPARE_AND_RECOVER";
            public const string CATEGORY = "CATEGORY";

            public const string CON = "CON";
            public const string CON_CAT = "CON_CAT";
            public const string CONTYPE = "CONTYPE";

            public const string STATE_NAME_ADJ = "STATE_NAME_ADJ";
            public const string STATE_ADJ = "STATE_ADJ";
            public const string ALL_SOLID = "ALL_SOLID";
            public const string SOLID = "SOLID";

            public const string GRASP = "GRASP";

            public const string HEAD = "HEAD";
            public const string LIMB = "LIMB";
            public const string DIGIT = "DIGIT";

            public const string NAME = "NAME";
            public const string LAYER = "LAYER";
        }

    }
}
