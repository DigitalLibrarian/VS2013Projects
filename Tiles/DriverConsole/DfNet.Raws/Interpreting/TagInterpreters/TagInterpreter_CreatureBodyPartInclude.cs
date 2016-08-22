using DfNet.Raws.Interpreting.Applicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_CreatureBodyPartInclude : IDfTagInterpreter
    {
        public string TagName
        {
            get { return DfTags.BODY; }
        }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            foreach (var p in tag.GetParams())
            {
                var defn = store.Get(DfTags.BODY, tag.GetParam(0));
                var app = new DfBodyPartIncludeApplicator(defn);
                app.Apply(store, context);
            }
        }
    }
}
