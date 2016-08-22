using DfNet.Raws.Interpreting.Applicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_ApplyCreatureVariation : IDfTagInterpreter
    {
        public string TagName
        {
            get { return DfTags.MiscTags.APPLY_CREATURE_VARIATION; }
        }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            var cvDefn = store.Get(DfTags.CREATURE_VARIATION, tag.GetParam(0));
            var app = new DfCreatureVariationApplicator(cvDefn, tag.GetParams().ToArray());
            app.Apply(store, context);
        }
    }
}
