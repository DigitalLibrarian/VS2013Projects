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
        public List<string> ApplyCreatureVariations { get; set; }

        public List<string> BodyPartSets { get; set; }

        public bool HasBody { get { return BodyPartSets.Any(); } }

        public List<Tag> Tokens { get; set; }
        public List<string> Castes = new List<string>();

        public Element Element { get; set; }

        public static Creature FromElement(Element ele)
        {
            var creature = new Creature
            {
                BodyPartSets = new List<string>(),
                ApplyCreatureVariations = new List<string>(),
                Tokens = new List<Tag>(),
                Element = ele
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
                        creature.ApplyCreatureVariations.Add(tag.Words[1]);
                        break;
                    case "CASTE":
                        creature.Castes.Add(tag.Words[1]);
                        break;
                }
                creature.Tokens.Add(tag);
            }
            return creature;
        }

        public static Creature FromElement(Element ele, string casteName)
        {
            var newEle = new Element { Tags = new List<Tag> { ele.Tags.First() } };

            var addCastes = new List<string>() { "ALL" };
            string parseCaste = "ALL";
            string nextLineCasteOnly = null;
            bool hit = false;

            foreach (var tag in ele.Tags)
            {
//[CASTE:<CASTE_NAME>] defines a caste called <CASTE_NAME>. Tags following this affect only this caste.

//[SELECT_CASTE:ALL] state the following tags affect all Castes

//[SELECT_CASTE:<CASTE_1>]

//[SELECT_ADDITIONAL_CASTE:<CASTE_2>]

//[SELECT_ADDITIONAL_CASTE:<CASTE_3>], etc., is used to specify that tags affect a subset of Castes                

                switch (tag.Name)
                {
                    case "CASTE":
                        hit = true;
                        nextLineCasteOnly = tag.Words[1];
                        break;
                    case "SELECT_CASTE":
                        parseCaste = tag.Words[1];
                        break;
                    case "SELECT_ADDITIONAL_CASTE":
                        addCastes.Add(tag.Words[1]);
                        break;
                }

                if (parseCaste == casteName || addCastes.Contains(parseCaste))
                {
                    if (nextLineCasteOnly != null)
                    {
                        if (hit || casteName == nextLineCasteOnly)
                        {
                            newEle.Tags.Add(tag.Clone());
                            if (!hit)
                            {
                                nextLineCasteOnly = null;
                            }
                            hit = false;
                        }
                    }
                    else
                    {
                        newEle.Tags.Add(tag.Clone());
                    }
                }
            }

            return FromElement(newEle);
        }

        public Creature Clone()
        {
            return new Creature
            {
                BodyPartSets = BodyPartSets.ToList(),
                ApplyCreatureVariations = ApplyCreatureVariations.ToList(),
                Tokens = Tokens.Select( x => x.Clone()).ToList(),
                ReferenceName = ReferenceName,
                Name = Name,
                NamePlural = NamePlural,
                NameAdjective = NameAdjective,
                Description = Description,
                CopyTagsFrom = CopyTagsFrom,
                Castes = Castes.ToList(),
                Element = Element.Clone()
            };
        }
    }
}
