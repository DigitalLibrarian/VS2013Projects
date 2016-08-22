using DfNet.Raws.Interpreting.TagInterpreters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.Applicators
{
    public class DfTissueApplicator : IContextApplicator
    {
        IDfObjectInterpreter Interpreter { get; set; }


        public DfTissueApplicator()
        {
            Interpreter = new DfObjectInterpreter(
                new TagInterpreter_UseTissueTemplate(),
                new TagInterpreter_AddTissue(),
                new TagInterpreter_RemoveTissue()
                );
        }

        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context, context.Source.Tags, true);
        }
    }
}
