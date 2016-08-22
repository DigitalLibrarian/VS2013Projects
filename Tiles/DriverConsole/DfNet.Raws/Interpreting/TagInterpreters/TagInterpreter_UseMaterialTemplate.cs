using DfNet.Raws.Interpreting.Applicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_UseMaterialTemplate : IDfTagInterpreter
    {
        public virtual string TagName
        {
            get { return DfTags.MiscTags.USE_MATERIAL_TEMPLATE; }
        }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            var numParams = tag.NumWords - 1;
            string templateName = null;
            string matName = null;
            if (numParams == 2)
            {
                matName = tag.GetParam(0);
                templateName = tag.GetParam(1);

            }
            else
            {
                templateName = tag.GetParam(0);
            }
            var defn = store.Get(DfTags.MATERIAL_TEMPLATE, templateName);
            var app = new DfMaterialTemplateApplicator(defn, matName);
            app.Apply(store, context);
        }
    }
}
