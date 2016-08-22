using DfNet.Raws.Interpreting.Applicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_BodyDetailPlanInclude : IDfTagInterpreter
    {
        public string TagName
        {
            get { return DfTags.BODY_DETAIL_PLAN; }
        }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            var bdpName = tag.GetParam(0);
            var defn = store.Get(DfTags.BODY_DETAIL_PLAN, bdpName);
            var app = new DfBodyDetailPlanApplicator(defn, tag.GetParams().ToArray());
            app.Apply(store, context);
        }
    }
}
