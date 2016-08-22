using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting
{
    public class DfObjectInterpreter : IDfObjectInterpreter
    {
        List<IDfTagInterpreter> TagInterpreters { get; set; }

        public DfObjectInterpreter(params IDfTagInterpreter[] tagInts)
        {
            TagInterpreters = tagInts.ToList();
        }
        
        public void Interpret(IDfObjectStore store, IDfObjectContext context, IList<DfTag> tags, bool insertMisses)
        {
            foreach (var tag in tags)
            {
                bool anyMatches = false;
                foreach (var tagInt in TagInterpreters)
                {
                    if (tag.Name.Equals(tagInt.TagName))
                    {
                        tagInt.Run(store, context, tag, tags);
                        anyMatches = true;
                    }
                }

                if(insertMisses && !anyMatches)
                {
                    context.InsertTags(tag);
                }
            }
        }
    }
}
