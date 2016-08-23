using DfNet.Raws.Interpreting;
using DfNet.Raws.Interpreting.Applicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws
{
    public class CreaturePipeline : IDfObjectPipeline
    {
        IDfObjectStore Store { get; set; }
        public CreaturePipeline(IDfObjectStore store)
        {
            Store = store;
        }

        public DfObject Run(string creatureName)
        {
            return _Run(creatureName);
        }

        public DfObject Run(string creatureName, string caste)
        {
            return _Run(creatureName, caste);
        }

        private DfObject _Run(string creatureName, string caste = null)
        {
            var parsedLeo = Store.Get(DfTags.CREATURE, creatureName);
            // TODO - all of this is stupid.  replace it with 
            // a simple param replacer method and delete all this crap
            var context = new DfObjectContext(parsedLeo);
            ApplyPass(new DfCreatureApplicator(parsedLeo), context);

            if (caste != null)
            {
                ApplyPass(new DfCasteApplicator(caste), context);
            }

            ApplyPass(new DfBodyApplicator(), context);
            ApplyPass(new DfMaterialApplicator(), context);
            ApplyPass(new DfTissueApplicator(), context);

            return context.Create();
        }

        void ApplyPass(IContextApplicator app, IDfObjectContext context)
        {
            context.StartPass();
            app.Apply(Store, context);
            context.EndPass();
        }
    }
}
