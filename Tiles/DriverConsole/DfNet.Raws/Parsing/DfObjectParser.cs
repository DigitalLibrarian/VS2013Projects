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
        DfTagParser TagParser { get; set; }

        public DfObjectParser() 
        {
            TagParser = new DfTagParser();
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
            for (int i = 0; i < tags.Count(); i++)
            {
                var tag = tags[i];
                if (IsObjectTag(tag) 
                    && objectTagNames.Any(name => name.StartsWith(tag.GetParam(0))))
                {
                    scannerWord = tag.GetParam(0);
                }else if(objectTagNames.Contains(tag.Name)
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
        }
    }
}
