using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_GoToStart : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.GO_TO_START; } }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            context.GoToStart();
        }
    }
}
