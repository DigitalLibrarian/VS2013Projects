using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Parsing
{
    public class DfObjectParser
    {
        IDfTagParser TagParser { get; set; }

        public DfObjectParser(IDfTagParser tagParser) 
        {
            TagParser = tagParser;
        }

        bool IsObjectTag(DfTag tag)
        {
            return (tag.Name.Equals(DfTags.OBJECT)
                        && tag.NumWords == 2);
        }

        public IEnumerable<DfObject> Parse(IEnumerable<string> lines, params string[] objectTagNames)
        {
            var tags = lines.SelectMany(TagParser.Parse).ToList();

            var scannerWord = "nerp";
            List<DfTag> workingSet = null;
            foreach(var tag in tags)
            {
                if (IsObjectTag(tag) 
                    && objectTagNames.Any(name => name.StartsWith(tag.GetParam(0))))
                {
                    scannerWord = tag.GetParam(0);
                }
                else if(objectTagNames.Contains(tag.Name)
                    && tag.Name.StartsWith(scannerWord))
                {
                    if (workingSet != null)
                    {
                        yield return new DfObject(workingSet);
                        workingSet = null;
                    }
                    workingSet = new List<DfTag> { tag };
                }
                else if (workingSet != null)
                {
                    workingSet.Add(tag);
                }
            }

            if (workingSet != null && workingSet.Any())
            {
                yield return new DfObject(workingSet);
            }
        }
    }
}
