using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    public class Inorganic
    {
        public const string TokenName = "INORGANIC";

        public string ReferenceName { get; set; }
        public List<Tag> Tokens { get; set; }

        public string AllSolidAdjective { get;  set; }
        public string UseMaterialTemplate { get; set; }

        public static Inorganic FromElement(Element ele)
        {
            var i = new Inorganic
            {
                Tokens = new List<Tag>()
            };

            foreach (var tag in ele.Tags)
            {
                if (tag.Name.Equals("STATE_NAME_ADJ") && tag.Words[1].Equals("ALL_SOLID"))
                {
                    i.AllSolidAdjective = tag.Words[2];
                }

                if (tag.Name.Equals("USE_MATERIAL_TEMPLATE"))
                {
                    i.UseMaterialTemplate = tag.Words[1];
                }

                if (tag.Name.Equals(ele.Name))
                {
                    i.ReferenceName = tag.Words[1];
                }

                i.Tokens.Add(tag);
            }

            return i;
        }
    }

}
