using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws
{
    public static class DfTags
    {
        public static readonly string OBJECT = "OBJECT";
        public static readonly string CREATURE = "CREATURE";
        public static readonly string CREATURE_VARIATION = "CREATURE_VARIATION";
        public static readonly string BODY = "BODY";
        public static readonly string BODY_DETAIL_PLAN = "BODY_DETAIL_PLAN";
        public static readonly string MATERIAL_TEMPLATE = "MATERIAL_TEMPLATE";
        public static readonly string TISSUE_TEMPLATE = "TISSUE_TEMPLATE";
        public static readonly string ITEM_WEAPON = "ITEM_WEAPON";

        public static string[] GetAllObjectTypes()
        {
            return new string[]{
                CREATURE,
                CREATURE_VARIATION,
                BODY,
                BODY_DETAIL_PLAN,
                MATERIAL_TEMPLATE,
                TISSUE_TEMPLATE,
                ITEM_WEAPON
            };
        }
    }
}
