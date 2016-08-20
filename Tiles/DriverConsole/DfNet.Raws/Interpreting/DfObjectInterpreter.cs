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

        // TODO - i don't like this here
        IEnumerable<string> FindCasteNames(DfObject dfObject)
        {
            return dfObject.Tags.Where(tag => tag.Name.Equals("CASTE"))
                .Select(tag => tag.GetParam(0));
        }

        public void Interpret(IDfObjectStore store, IDfObjectContext context, IList<DfTag> tags)
        {
            foreach (var tag in tags)
            {
                foreach (var tagInt in TagInterpreters)
                {
                    if (tag.Name.Equals(tagInt.TagName))
                    {
                        tagInt.Run(store, context, tag, tags);
                    }
                }
            }
        }
    }
}
