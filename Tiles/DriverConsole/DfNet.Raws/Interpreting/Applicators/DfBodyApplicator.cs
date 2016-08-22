using DfNet.Raws.Interpreting.TagInterpreters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.Applicators
{
    public class DfBodyApplicator : IContextApplicator
    {
        IDfObjectInterpreter Interpreter { get; set; }
        public DfBodyApplicator()
        {
            Interpreter = new DfObjectInterpreter(
                new TagInterpreter_CreatureBodyPartInclude(),
                new TagInterpreter_BodyDetailPlanInclude()
                );
        }

        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context, context.Source.Tags, true);
        }
    }
}
