using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting
{
    public class DfBodyPartIncludeApplicator : IContextApplicator
    {
        DfObject Defn { get; set; }
        IDfObjectInterpreter Interpreter { get; set; }

        public DfBodyPartIncludeApplicator(DfObject defn) 
        {
            Defn = defn;
            Interpreter = new DfObjectInterpreter(GetBodyTagInterpreters());
        }

        private IDfTagInterpreter[] GetBodyTagInterpreters()
        {
            return new IDfTagInterpreter[]{
            };
        }
        
        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context, Defn.Tags, true);
        }
    }
}
