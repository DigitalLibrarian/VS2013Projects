using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using DwarfFortressNet.RawModels;
using Tiles.Bodies;
using DwarfFortressNet.Bridge;
namespace DwarfFortressNet.RawParserConsole
{
    class Program
    {
        private static readonly string _DirKey = @"DwarfFortressRawsDirectory";
        static void Main(string[] args)
        {
            var dirStr = System.Configuration.ConfigurationManager.AppSettings.Get(_DirKey);

            foreach (var file in AllFiles(dirStr))
            {
                Consume(AllLinesInFile(file.FullName));
            }

            var objDb = new ObjectDb();
            foreach (var pair in Elements)
            {
                Console.WriteLine(string.Format("{0}: {1}", pair.Key, pair.Value.Count()));
                if (pair.Key == BodyPartSet.TokenName)
                {
                    foreach (var ele in pair.Value)
                    {
                        var bodyPartSet = BodyPartSet.FromElement(ele);
                        objDb.Add(bodyPartSet.ReferenceName, bodyPartSet);
                    }
                }
                else if (pair.Key == BodyDetailPlan.TokenName)
                {
                    foreach (var ele in pair.Value)
                    {
                        var plan = BodyDetailPlan.FromElement(ele);
                        objDb.Add(plan.ReferenceName, plan);
                    }
                }
                else if (pair.Key == Creature.TokenName)
                {
                    foreach (var ele in pair.Value)
                    {
                        var creature = Creature.FromElement(ele);
                        objDb.Add(creature.ReferenceName, creature);
                    }

                }
            }



            var c = objDb.Get<Creature>("GOBLIN");
            var body = CreateBody(c, objDb);
            System.Console.ReadKey();
        }

        static IBody CreateBody(Creature c, ObjectDb objDb)
        {
            return DfCreatureBodyBuilder.FromCreatureDefinition(c, objDb);
        }

        static Dictionary<string, List<Element>> Elements = new Dictionary<string, List<Element>>();

        static void Consume(IEnumerable<string> lines)
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
                        var eles = ExtractElements(secondWord, tags).ToList();
                        foreach (var ele in eles)
                        {
                            if (!Elements.ContainsKey(secondWord))
                            {
                                Elements.Add(secondWord, new List<Element> { ele });
                            }
                            else
                            {
                                Elements[ele.Name].Add(ele);
                            }
                        }
                    }
                }
            }
        }

        static IEnumerable<Tag> ExtractTags(string line)
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

                line = line.Substring(rightBracket+1);
            }
        }

        static IEnumerable<Element> ExtractElements(string elementName, IEnumerable<Tag> tags)
        {
            Element currEle = null;
            foreach (var tag in tags)
            {
                if (tag.Words[0].Equals(elementName))
                {
                    if (currEle == null)
                    {
                        currEle = new Element
                        {
                            Name = elementName,
                            Tags = new List<Tag> { tag }
                        };
                    }
                    else
                    {
                        yield return currEle;

                        currEle = new Element
                        {
                            Name = elementName,
                            Tags = new List<Tag> {  tag}
                        };
                    }
                }
                else if(currEle != null)
                {
                    currEle.Tags.Add(tag);
                }
            }
            if (currEle != null && currEle.Tags.Any())
            {
                yield return currEle;
            }
        }

        static Regex TagRegex()
        {
            return new Regex(Regex.Escape("[") + "((.+):)+(.+)]");
        }

        static bool HasTag(string line)
        {
            //[APPLY_CREATURE_VARIATION:STANDARD_CLIMBING_GAITS:6561:6115:5683:1755:7456:8567] 5 kph
            return TagRegex().IsMatch(line);
        }


        static IEnumerable<FileInfo> AllFiles(string dirStr)
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

        static IEnumerable<string> AllLinesInFile(string file)
        {
            return File.ReadLines(file);
        }

        public static object Dictionary { get; set; }
    }
}
