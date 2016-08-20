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

        public IEnumerable<DfObject> Parse(IEnumerable<string> lines, params string[] objectTagNames)
        {
            List<DfTag> workingSet = null;
            foreach (var line in lines)
            {
                foreach (var tag in TagParser.Parse(line))
                {
                    if (objectTagNames.Contains(tag.Name))
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
}
