using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.Applicators
{
    public class DfBodyDetailPlanApplicator : IContextApplicator
    {
        DfObject Defn { get; set; }
        string[] Args { get; set; }
        public DfBodyDetailPlanApplicator(DfObject defn, params string[] args) 
        {
            Defn = defn;
            Args = args;
        }

        private IDfTagInterpreter[] GetTagInterpreters()
        {
            return new IDfTagInterpreter[]{};
        }

        // TODO - this is duplicated in creature variations, move to DfTag
        string[] ApplyArgs(string[] p)
        {
            var newParams = new List<string>();

            foreach (var pIn in p)
            {
                if (Args.Count() > 1 && pIn.StartsWith("ARG"))
                {
                    int index = int.Parse(pIn.Substring(3));
                    newParams.Add(Args[index]);
                }
                else
                {
                    newParams.Add(pIn);
                }
            }

            return newParams.ToArray();
        }

        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            var newTags = Defn.Tags.Skip(1).Select(x =>
                new DfTag(x.Name, ApplyArgs(x.GetParams())));

            context.InsertTags(newTags.ToArray());
        }
    }
}
