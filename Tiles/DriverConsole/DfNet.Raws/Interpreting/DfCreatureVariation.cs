using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting
{
    public class DfCreatureVariation
    {
        DfObject CvDefn { get; set; }
        IDfObjectInterpreter Interpreter { get; set; }

        public DfCreatureVariation(DfObject cvDefn) 
        {
            CvDefn = cvDefn;
            Interpreter = new DfObjectInterpreter(GetCreatureVariationTagInterpreters());
        }

        private static IDfTagInterpreter[] GetCreatureVariationTagInterpreters()
        {
            return new IDfTagInterpreter[]{
                new TagInterpreter_CvNewTag(),
                new TagInterpreter_CvConvertTag(),
                new TagInterpreter_CvRemoveTag()
            };
        }
        
        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context, CvDefn.Tags);
        }
    }
}
