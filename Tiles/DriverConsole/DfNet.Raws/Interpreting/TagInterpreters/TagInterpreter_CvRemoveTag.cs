using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_CvRemoveTag : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.CV_REMOVE_TAG; } }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            context.RemoveTagsByName(tag.GetParam(0));
        }
    }
}
