using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_CvNewTag : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.CV_NEW_TAG; } }

        string[] Args { get; set; }

        public TagInterpreter_CvNewTag(string[] args)
        {
            Args = args;
        }

        string[] ApplyArgs(string[] p)
        {
            var newParams = new List<string>();

            foreach (var pIn in p)
            {
                if (Args.Count() > 1 && pIn.StartsWith("!ARG"))
                {
                    int index = int.Parse(pIn.Substring(4));
                    newParams.Add(Args[index]);
                }
                else
                {
                    newParams.Add(pIn);
                }
            }

            return newParams.ToArray();
        }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            var p = tag.GetParams();
            p = ApplyArgs(p);
            var newTag = new DfTag(p);
            context.InsertTags(newTag);
        }
    }
}
