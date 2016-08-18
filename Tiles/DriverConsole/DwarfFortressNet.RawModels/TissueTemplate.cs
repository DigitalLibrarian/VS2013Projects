using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    public class TissueTemplate
    {
        public const string TokenName = "TISSUE_TEMPLATE";

        public string ReferenceName { get; set; }
        public List<Tag> Tokens { get; set; }

        public string GameName { get; set; }
        public string GameNamePlural { get; set; }
        public int RelativeThickness { get; set; }
        public string TissueStrategy { get; set; }
        public string TissueRefName { get; set; }

        public static TissueTemplate FromElement(Element ele)
        {
            var tt = new TissueTemplate
            {
                Tokens = new List<Tag>(),
                ReferenceName = ele.Tags.First().Words[1]
            };
            foreach (var tag in ele.Tags)
            {
                tt.Tokens.Add(tag);

                switch (tag.Name)
                {
                    case "TISSUE_NAME" :
                        tt.GameName = tag.Words[1];
                        tt.GameNamePlural = tag.Words[2];
                        break;
                    case "RELATIVE_THICKNESS":
                        tt.RelativeThickness = int.Parse(tag.Words[1]);
                        break;
                    case "TISSUE_MATERIAL":
                        tt.TissueStrategy = tag.Words[1];
                        tt.TissueRefName = tag.Words[2];
                        break;
                }
            }
            return tt;
        }
    }
}
