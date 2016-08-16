using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    public class BodyPartSet
    {
        public static readonly string TokenName = "BODY";

        public string ReferenceName { get; set; }
        public List<BodyPart> BodyParts { get; set; }

        public static BodyPartSet FromElement(Element ele)
        {
            var referenceName = ele.Tags.First().Words[1];

            List<Tag> bodyPartTags = null;
            List<BodyPart> parts = new List<BodyPart>();
            foreach (var tag in ele.Tags)
            {
                if (tag.Name == BodyPart.TokenName)
                {
                    if (bodyPartTags != null)
                    {
                        parts.Add(BodyPart.FromTags(bodyPartTags));

                    }
                    bodyPartTags = new List<Tag> { tag };
                }
                else if (bodyPartTags != null)
                {
                    bodyPartTags.Add(tag);
                }
            }


            if (bodyPartTags != null)
            {
                parts.Add(BodyPart.FromTags(bodyPartTags));

            }
            return new BodyPartSet
            {
                ReferenceName = referenceName,
                BodyParts = parts
            };
        }
    }
}
