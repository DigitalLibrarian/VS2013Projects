using DfNet.Raws.Interpreting.TagInterpreters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.Applicators
{
    public class DfMaterialApplicator : IContextApplicator
    {
        IDfObjectInterpreter Interpreter { get; set; }

        public DfMaterialApplicator()
        {
            Interpreter = new DfObjectInterpreter(
                new TagInterpreter_UseMaterialTemplate(),
                new TagInterpreter_AddMaterial(),
                new TagInterpreter_RemoveMaterial()
                );
        }

        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context, context.Source.Tags, true);
        }
    }
}
