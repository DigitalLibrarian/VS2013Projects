using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{

    //[CREATURE_VARIATION: ANIMAL_PERSON]
    //    [CV_REMOVE_TAG: NAME]

    //[CV_CONVERT_TAG]
    //    [CVCT_MASTER: BODY]
    //    [CVCT_TARGET: QUADRUPED]
    //    [CVCT_REPLACEMENT: HUMANOID]

    //[CV_NEW_TAG: LOCAL_POPS_PRODUCE_HEROES]


    public class CreatureVariation
    {
        public const string TokenName = "CREATURE_VARIATION";

        public string ReferenceName { get; set; }

        public List<string> CvRemoveTags = new List<string>();
        public List<Tag> CvNewTags = new List<Tag>();
        public List<Tag> Tokens = new List<Tag>();
        public List<CvConvertTag> CvConvertTags = new List<CvConvertTag>();

        public static CreatureVariation FromElement(Element ele)
        {
            var cv = new CreatureVariation();

            bool wasConverBreak = false;
            CvConvertTag convertTag = null;
            foreach (var tag in ele.Tags)
            {
                wasConverBreak = false;
                switch (tag.Name)
                {
                    case TokenName:
                        cv.ReferenceName = tag.Words[1];
                        break;

                    case "CV_REMOVE_TAG":
                        wasConverBreak = true;
                        cv.CvRemoveTags.Add(tag.Words[1]);
                        break;

                    case "CV_NEW_TAG":
                        wasConverBreak = true;
                        var newTag = new Tag{
                            Words = tag.Words.Skip(1).ToArray()
                        };
                        cv.CvNewTags.Add(newTag);
                        break;

                    case "CV_CONVERT_TAG":
                        wasConverBreak = convertTag != null;
                        convertTag = new CvConvertTag();
                        break;

                    case "CVCT_MASTER":
                        convertTag.Master = string.Join(":", tag.Words.Skip(1));
                        break;

                    case "CVCT_TARGET":
                        convertTag.Target = string.Join(":", tag.Words.Skip(1));
                        break;

                    case "CVCT_REPLACEMENT":
                        convertTag.Replacement = tag.Words.Skip(1).ToList();
                        break;
                }
                if (convertTag != null && wasConverBreak)
                {
                    cv.CvConvertTags.Add(convertTag);
                }
            }

            return cv;
        }

        public class CvConvertTag
        {
            public string Target { get; set; }
            public string Master { get; set; }
            public List<string> Replacement { get; set; }
        }
    }
}
