using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_RemoveTissue : IDfTagInterpreter
    {
        public string TagName
        {
            get { return DfTags.MiscTags.REMOVE_TISSUE; }
        }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            var working = context.WorkingSet.ToList();
            var tissueIndex = working.FindIndex(t => t.Name.Equals(DfTags.MiscTags.START_TISSUE)
                     && t.GetParam(0).Equals(tag.GetParam(0)));
            var runLength = working.FindIndex(tissueIndex,
                t => t.Name.Equals(DfTags.MiscTags.END_TISSUE)
                     && t.GetParam(0).Equals(tag.GetParam(0)));
            var removals = working.GetRange(tissueIndex, runLength - tissueIndex + 1);

            context.Remove(removals.ToArray());
        }
    }
}
