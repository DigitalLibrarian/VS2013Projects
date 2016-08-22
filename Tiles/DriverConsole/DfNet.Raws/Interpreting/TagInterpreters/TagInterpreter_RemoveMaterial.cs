using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_RemoveMaterial : IDfTagInterpreter
    {
        public string TagName
        {
            get { return DfTags.MiscTags.REMOVE_MATERIAL; }
        }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            var working = context.WorkingSet.ToList();
            var matIndex = working.FindIndex(t => t.Name.Equals(DfTags.MiscTags.START_MATERIAL)
                     && t.GetParam(0).Equals(tag.GetParam(0)));
            var runLength = working.FindIndex(matIndex,
                t => t.Name.Equals(DfTags.MiscTags.END_MATERIAL)
                     && t.GetParam(0).Equals(tag.GetParam(0)));
            var removals = working.GetRange(matIndex, runLength - matIndex + 1);

            context.Remove(removals.ToArray());
        }
    }
}
