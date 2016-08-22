using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    //[CV_CONVERT_TAG]
    //    [CVCT_MASTER:BODY]
    //    [CVCT_TARGET:QUADRUPED_NECK]
    //    [CVCT_REPLACEMENT:HUMANOID_NECK:3FINGERS]
    public class TagInterpreter_CvConvertTag : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.CV_CONVERT_TAG; } }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag triggerTag, IList<DfTag> tags)
        {
            int startIndex = tags.IndexOf(triggerTag);
            var rest = tags.Skip(startIndex + 1).ToList();
            int endIndex = rest.FindIndex(x => x.Name.Equals(this.TagName));
            if (endIndex == -1)
            {
                endIndex = rest.Count();
            }
            rest = rest.Take(endIndex).ToList();

            var masterTag = rest.Single(t => t.Name.Equals(DfTags.MiscTags.CVCT_MASTER));
            var targetTag = rest.Single(t => t.Name.Equals(DfTags.MiscTags.CVCT_TARGET));
            var replaceTag = rest.SingleOrDefault(t => t.Name.Equals(DfTags.MiscTags.CVCT_REPLACEMENT));

            var targetWords = targetTag.GetParams();
            var replacementWords = replaceTag == null ? new string[] { } : replaceTag.GetParams();

            foreach (var master in
                context.WorkingSet.Where(t => t.Name.Equals(masterTag.GetParam(0))
                    && t.GetWords().Any(word => targetWords.Contains(word))))
            {

                var newParams = new List<string>();
                bool once = true;
                foreach (var word in master.GetParams())
                {
                    if (targetWords.Contains(word))
                    {
                        if (once)
                        {
                            if (replacementWords.Any())
                            {
                                newParams.AddRange(replacementWords);
                            }
                            once = false;
                        }
                    }
                    else
                    {
                        newParams.Add(word);
                    }
                }

                if (newParams.Any())
                {
                    context.ReplaceTag(master, new DfTag(master.Name, newParams.ToArray()));
                }
            }
        }
    }
}
