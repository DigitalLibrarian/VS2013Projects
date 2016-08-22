using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_GoToTag : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.GO_TO_TAG; } }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            context.GoToTag(tag.GetParam(0));
        }
    }
}
