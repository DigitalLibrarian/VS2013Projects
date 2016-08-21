using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting
{
    public class DfCreatureApplicator : IContextApplicator
    {
        DfObject CDefn { get; set; }
        IDfObjectInterpreter Interpreter { get; set; }
        public DfCreatureApplicator(DfObject cDefn)
        {
            CDefn = cDefn;

            Interpreter = new DfObjectInterpreter(
                new TagInterpreter_CopyTagsFrom(),
                new TagInterpreter_GoToStart(),
                new TagInterpreter_GoToEnd(),
                new TagInterpreter_GoToTag(),
                new TagInterpreter_ApplyCreatureVariation()
                //new TagInterpreter_CreatureBodyPartInclude(),
                //new TagInterpreter_BodyDetailPlanInclude()
                );
        }

        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context,  context.Source.Tags, true);
        }
    }
}
