using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting
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

        public static IEnumerable<string> FindRequiredCVs(DfObject dfObj)
        {
            foreach (var tag in dfObj.Tags)
            {
                if (tag.Name.Equals(DfTags.MiscTags.APPLY_CREATURE_VARIATION))
                {
                    yield return tag.GetParam(0);
                }
            }
        }

        public static IEnumerable<string> FindRequiredCreature(DfObject dfObj)
        {
            foreach (var tag in dfObj.Tags)
            {
                if (tag.Name.Equals(DfTags.MiscTags.COPY_TAGS_FROM))
                {
                    yield return tag.GetParam(0);
                }
            }

        }
    }
}
