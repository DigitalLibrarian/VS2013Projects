using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.Applicators
{
    public class DfBodyPartIncludeApplicator : IContextApplicator
    {
        DfObject Defn { get; set; }
        Dictionary<string, List<DfTag>> BPTags { get; set; }
        Dictionary<string, int> BPRelSize { get; set; }


        public DfBodyPartIncludeApplicator(DfObject defn) 
        {
            Defn = defn;
        }



        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            bool keeper = false;

            foreach (var tag in Defn.Tags)
            {

                if (tag.Name.Equals(DfTags.MiscTags.BP))
                {
                    if (!context.WorkingSet.Any(
                        t => t.Name.Equals(DfTags.MiscTags.BP)
                            && t.GetParam(0).Equals(tag.GetParam(0))
                        ))
                    {
                        keeper = true;
                    }
                    else
                    {
                        keeper = false;
                    }
                }


                if (keeper)
                {
                    context.InsertTags(tag);
                }
            }

        }
    }
}
