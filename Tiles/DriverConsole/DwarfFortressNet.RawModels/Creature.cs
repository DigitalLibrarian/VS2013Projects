using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    public class Creature
    {
        public const string TokenName = "CREATURE";

        public string ReferenceName { get; set; }
        public string Name { get; set; }
        public string NamePlural { get; set; }
        public string NameAdjective { get; set; }
        public string Description { get; set; }

        public string CopyTagsFrom { get; set; }
        public List<string> CreatureVariations { get; set; }

        public List<string> BodyPartSets { get; set; }
        public List<string> BodyDetailPlans { get; set; }
        public List<Tag> Tokens { get; set; }


        public static Creature FromElement(Element ele)
        {
            var creature = new Creature
            {
                BodyPartSets = new List<string>(),
                BodyDetailPlans = new List<string>(),
                CreatureVariations = new List<string>(),
                Tokens = new List<Tag>()
            };

            foreach (var tag in ele.Tags)
            {
                switch (tag.Name)
                {
                    case "CREATURE":
                        creature.ReferenceName = tag.Words[1];
                        break;
                    case "NAME":
                        creature.Name = tag.Words[1];
                        creature.NamePlural = tag.Words[2];
                        creature.NameAdjective = tag.Words[3];
                        break;
                    case "BODY_DETAIL_PLAN":
                        creature.BodyDetailPlans.Add(tag.Words[1]);
                        break;
                    case "BODY":
                        foreach (var word in tag.Words.Skip(1))
                        {
                            creature.BodyPartSets.Add(word);
                        }
                        break;
                    case "DESCRIPTION":
                        creature.Description = tag.Words[1];
                        break;
                    case "COPY_TAGS_FROM":
                        creature.CopyTagsFrom = tag.Words[1];
                        break;
                    case "APPLY_CREATURE_VARIATION":
                        creature.CreatureVariations.Add(tag.Words[1]);
                        break;
                }
                creature.Tokens.Add(tag);
            }
            return creature;
        }
    }
}
