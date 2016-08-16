using DwarfFortressNet.RawModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Items;

namespace DwarfFortressNet.Bridge
{
    public class DfFabricator
    {
        Dictionary<string, List<Element>> Elements = new Dictionary<string, List<Element>>();
        ObjectDb Db = new ObjectDb();
        
        public IEnumerable<ItemWeapon> ItemWeapons
        {
            get
            {
                return Db.Get<ItemWeapon>();
            }
        }

        public IEnumerable<Inorganic> Inorganics
        {
            get
            {
                return Db.Get<Inorganic>();
            }
        }

        public IEnumerable<Creature> Creatures
        {
            get { return Db.Get<Creature>(); }
        }

        public IEnumerable<MaterialTemplate> MaterialTemplates
        {
            get { return Db.Get<MaterialTemplate>(); }
        }

        public void ReadDfRawDir(string dir)
        {
            foreach (var file in AllFiles(dir))
            {
                Consume(AllLinesInFile(file.FullName));
            }

            foreach (var pair in Elements)
            {
                Console.WriteLine(string.Format("{0}: {1}", pair.Key, pair.Value.Count()));
                if (pair.Key == BodyPartSet.TokenName)
                {
                    foreach (var ele in pair.Value)
                    {
                        var bodyPartSet = BodyPartSet.FromElement(ele);
                        Db.Add(bodyPartSet.ReferenceName, bodyPartSet);
                    }
                }
                else if (pair.Key == BodyDetailPlan.TokenName)
                {
                    foreach (var ele in pair.Value)
                    {
                        var plan = BodyDetailPlan.FromElement(ele);
                        Db.Add(plan.ReferenceName, plan);
                    }
                }
                else if (pair.Key == Creature.TokenName)
                {
                    foreach (var ele in pair.Value)
                    {
                        var creature = Creature.FromElement(ele);
                        Db.Add(creature.ReferenceName, creature);
                    }
                }
                else if (pair.Key == MaterialTemplate.TokenName)
                {
                    foreach (var ele in pair.Value)
                    {
                        var matTemplate = MaterialTemplate.FromElement(ele);
                        Db.Add(matTemplate.ReferenceName, matTemplate);
                    }
                }
                else if (pair.Key == ItemWeapon.TokenName)
                {
                    foreach (var ele in pair.Value)
                    {
                        var obj = ItemWeapon.FromElement(ele);
                        Db.Add(obj.ReferenceName, obj);
                    }
                }
            }

            foreach (var ele in Elements.Where(pair => pair.Key == Inorganic.TokenName).SelectMany(x => x.Value))
            {
                var inorg = Inorganic.FromElement(ele);
                Db.Add(inorg.ReferenceName, inorg);
            }

        }

        void Consume(IEnumerable<string> lines)
        {
            var tags = new List<Tag>();
            foreach (var line in lines)
            {
                if (HasTag(line))
                {
                    tags.AddRange(ExtractTags(line));
                }
            }

            if (tags.Any())
            {
                var firstTag = tags.First();
                if (firstTag.Words.Count() == 2)
                {
                    var firstWord = firstTag.Words[0];
                    var secondWord = firstTag.Words[1];

                    if (firstWord == "OBJECT")
                    {
                        var scanList = new List<string> { secondWord };
                        if (secondWord == "ITEM")
                        {
                            scanList = new List<string>{
                                "ITEM_ARMOR",
                                "ITEM_TOOL",
                                "ITEM_PANTS",
                                "ITEM_TOOL",
                                "ITEM_SHIELD",
                                "ITEM_GLOVES",
                                "ITEM_WEAPON",
                                "ITEM_HELM"
                            };
                        }

                        foreach (var scanWord in scanList)
                        {
                            var eles = ExtractElements(scanWord, tags).ToList();
                            foreach (var ele in eles)
                            {
                                if (!Elements.ContainsKey(scanWord))
                                {
                                    Elements.Add(scanWord, new List<Element> { ele });
                                }
                                else
                                {
                                    Elements[scanWord].Add(ele);
                                }
                            }
                        }
                    }
                }
            }
        }

        IEnumerable<Tag> ExtractTags(string line)
        {
            while (line.Contains('['))
            {
                var leftBracket = line.IndexOf('[');
                var rightBracket = line.IndexOf(']');
                var diff = rightBracket - leftBracket;
                var dataStr = line.Substring(leftBracket + 1, diff - 1);

                yield return new Tag
                {
                    Words = dataStr.Split(':').ToList()
                };

                line = line.Substring(rightBracket + 1);
            }
        }

        IEnumerable<Element> ExtractElements(string elementName, IEnumerable<Tag> tags)
        {
            var eles = new List<Element>();

            Element currEle = null;
            foreach (var tag in tags)
            {
                var tagName = tag.Name;
                if (tagName.Equals(elementName))
                {
                    if (currEle == null)
                    {
                        currEle = new Element
                        {
                            Name = elementName,
                            Tags = new List<Tag> { }
                        };
                    }
                    else
                    {
                        eles.Add(currEle);

                        currEle = new Element
                        {
                            Name = elementName,
                            Tags = new List<Tag> { }
                        };

                    }
                }

                if (currEle != null)
                {
                    currEle.Tags.Add(tag);
                }
                
            }
            if (currEle != null && currEle.Tags.Any())
            {
                eles.Add(currEle);
            }

            return eles;
        }

        Regex TagRegex()
        {
            return new Regex(Regex.Escape("[") + ".+?]");
        }

        bool HasTag(string line)
        {
            //[APPLY_CREATURE_VARIATION:STANDARD_CLIMBING_GAITS:6561:6115:5683:1755:7456:8567] 5 kph
            return TagRegex().IsMatch(line);
        }

        IEnumerable<FileInfo> AllFiles(string dirStr)
        {
            var dirInfo = new DirectoryInfo(dirStr);
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                yield return fileInfo;
            }

            foreach (var babyDirInfo in dirInfo.GetDirectories())
            {
                foreach (var babyFileInfo in AllFiles(babyDirInfo.FullName))
                {
                    yield return babyFileInfo;
                }
            }
        }

        IEnumerable<string> AllLinesInFile(string file)
        {
            return File.ReadLines(file);
        }


        public IBody CreateBody(Creature c)
        {
            return DfCreatureBodyBuilder.FromCreatureDefinition(c, Db);
        }

        public IItem CreateWeapon(Inorganic inorg, ItemWeapon weapon)
        {
            return DfWeaponItemBuilder.FromDefinition(inorg, weapon, Db);
        }
    }
}
