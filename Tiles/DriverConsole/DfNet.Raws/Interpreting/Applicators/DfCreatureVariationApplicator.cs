using DfNet.Raws.Interpreting.TagInterpreters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.Applicators
{
    public class DfCreatureVariationApplicator : IContextApplicator
    {
        DfObject CvDefn { get; set; }
        IDfObjectInterpreter Interpreter { get; set; }
        string[] Args { get; set; }
        public DfCreatureVariationApplicator(DfObject cvDefn, params string[] args) 
        {
            CvDefn = cvDefn;
            Args = args;
            Interpreter = new DfObjectInterpreter(GetCreatureVariationTagInterpreters());
        }

        private IDfTagInterpreter[] GetCreatureVariationTagInterpreters()
        {
            return new IDfTagInterpreter[]{
                new TagInterpreter_CvRemoveTag(),
                new TagInterpreter_CvNewTag(Args),
                new TagInterpreter_CvConvertTag()
            };
        }
        
        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context, CvDefn.Tags, false);
        }
    }
}
