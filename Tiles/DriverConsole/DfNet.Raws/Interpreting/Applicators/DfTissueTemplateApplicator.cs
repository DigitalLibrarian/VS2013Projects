using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.Applicators
{
    public class DfTissueTemplateApplicator : IContextApplicator
    {
        DfObject Defn { get; set; }
        string Name { get; set; }
        IDfObjectInterpreter Interpreter { get; set; }
        public DfTissueTemplateApplicator(DfObject defn, string name)
        {
            Defn = defn;
            Name = name;

            Interpreter = new DfObjectInterpreter();
        }

        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            DfTag startTag = new DfTag(DfTags.MiscTags.START_TISSUE, Name);
            var newTags = new List<DfTag> { startTag };

            newTags.AddRange(Defn.Tags.Where(x => !(x.Name.Equals(DfTags.TISSUE_TEMPLATE)
                && x.GetParam(0).Equals(Name))));

            DfTag endTag =  new DfTag(DfTags.MiscTags.END_TISSUE, Name);
            newTags.Add(endTag);

            Interpreter.Interpret(store, context, newTags, true);
        }
    }
}
