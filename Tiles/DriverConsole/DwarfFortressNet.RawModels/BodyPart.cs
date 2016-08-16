using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    //[BP:RA:right front leg:STP][CONTYPE:UPPERBODY][LIMB][RIGHT][CATEGORY:LEG_FRONT]
    //[DEFAULT_RELSIZE:900]
    public class BodyPart
    {
        public static readonly string TokenName = "BP";

        public string ReferenceName { get; set; }

        public string Name { get; set; }
        public string NamePlural { get; set; }
        /// <summary>
        /// Reference name of specific body part that this are connected to
        /// </summary>
        public string Con { get; set; }
        /// <summary>
        /// A category of body parts that this is connected to
        /// </summary>
        public string ConType { get; set; }
        public int DefaultRelativeSize { get; set; }
        public string Category { get; set; }

        public List<Tag> Tokens { get; set; }
        
        public static BodyPart FromTags(IList<Tag> tags)
        {
            var bp = new BodyPart
            {
                Tokens = new List<Tag>()
            };
            foreach (var tag in tags)
            {
                switch (tag.Name)
                {
                    case "BP":
                        bp.ReferenceName = tag.Words[1];
                        bp.Name = tag.Words[2];
                        bp.NamePlural = tag.Words[3] == "STP" ? STP(tag.Words[3]) : tag.Words[3];
                        break;
                    case "CONTYPE":
                        bp.ConType = tag.Words[1];
                        break;
                    case "CON":
                        bp.Con = tag.Words[1];
                        break;
                    case "CATEGORY":
                        bp.Category = tag.Words[1];
                        break;
                    case "DEFAULT_RELSIZE":
                        bp.DefaultRelativeSize = int.Parse(tag.Words[1]);
                        break;
                    default:
                        bp.Tokens.Add(tag);
                        break;
                }
            }
            return bp;
        }


        static string STP(string s)
        {
            return string.Format("{0}s", s);
        }
    }
}
