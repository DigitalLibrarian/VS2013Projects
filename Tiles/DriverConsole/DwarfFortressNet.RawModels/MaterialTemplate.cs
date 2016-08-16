using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    public class MaterialTemplate
    {
        public const string TokenName = "MATERIAL_TEMPLATE";

        public string ReferenceName { get; set; }
        public List<Tag> Tokens { get; set; }

        public string AllSolidAdjective
        {
            get
            {
                if (StateInfo.ContainsKey("STATE_NAME_ADJ"))
                {
                    if (StateInfo["STATE_NAME_ADJ"].ContainsKey("ALL_SOLID"))
                    {
                        return StateInfo["STATE_NAME_ADJ"]["ALL_SOLID"];
                    }
                }
                return null;
            }
        }

        public IDictionary<string, IDictionary<string, string>> StateInfo { get; set; }

        void Add(string tag, string name, string value)
        {
            if (StateInfo.ContainsKey(tag))
            {
                StateInfo[tag][name] = value;
            }
            else
            {
                StateInfo[tag] = new Dictionary<string, string>{
                    {name, value}
                };
            }
        }

        public static MaterialTemplate FromElement(Element ele)
        {
            var mt = new MaterialTemplate
            {
                StateInfo = new Dictionary<string, IDictionary<string, string>>(),
                Tokens = new List<Tag>()
            };

            foreach (var tag in ele.Tags)
            {
                if (tag.Name.StartsWith("STATE_"))
                {
                    mt.Add(tag.Name, tag.Words[1], tag.Words[2]);
                }

                if (tag.Name.Equals(ele.Name))
                {
                    mt.ReferenceName = tag.Words[1];
                }

                mt.Tokens.Add(tag);
            }

            return mt;
        }
    }
}
