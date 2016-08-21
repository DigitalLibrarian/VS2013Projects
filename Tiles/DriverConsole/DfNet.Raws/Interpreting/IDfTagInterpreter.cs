using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting
{
    public interface IDfTagInterpreter
    {
        string TagName { get; }

        void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags);
    }


    public class TagInterpreter_GoToStart : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.GO_TO_START; } }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            context.GoToStart();
        }
    }

    public class TagInterpreter_GoToEnd : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.GO_TO_END; } }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            context.GoToEnd();
        }
    }

    public class TagInterpreter_GoToTag : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.GO_TO_TAG; } }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            context.GoToTag(tag.GetParam(0));
        }
    }

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

    public class TagInterpreter_CvRemoveTag : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.CV_REMOVE_TAG; } }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            context.RemoveTagsByName(tag.GetParam(0));
        }
    }

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
            var rest = tags.Skip(startIndex+1).ToList();
            int endIndex = rest.FindIndex(x => x.Name.Equals(this.TagName));
            if (endIndex == -1)
            {
                endIndex = rest.Count();
            }
            rest = rest.Take(endIndex).ToList();

            var masterTag = rest.Single(t => t.Name.Equals(DfTags.MiscTags.CVCT_MASTER));
            var targetTag = rest.Single(t => t.Name.Equals(DfTags.MiscTags.CVCT_TARGET));
            var replaceTag = rest.Single(t => t.Name.Equals(DfTags.MiscTags.CVCT_REPLACEMENT));

            var targetWords = targetTag.GetParams();
            var replacementWords = replaceTag.GetParams();

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
                            newParams.AddRange(replacementWords);
                            once = false;
                        }
                    }
                    else
                    {
                        newParams.Add(word);
                    }
                }
                
                if(newParams.Any())
                {
                    context.ReplaceTag(master, new DfTag(master.Name, newParams.ToArray()));
                }
            }
        }
    }

    public class TagInterpreter_CopyTagsFrom : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.COPY_TAGS_FROM; } }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag triggerTag, IList<DfTag> tags)
        {
            var creatureType = triggerTag.GetParam(0);
            var creatureDf = store.Get(DfTags.CREATURE, creatureType);

            context.CopyTagsFrom(creatureDf);
        }
    }

    public class Taginterpreter_ApplyCreatureVariation : IDfTagInterpreter
    {
        public string TagName
        {
            get { return DfTags.MiscTags.APPLY_CREATURE_VARIATION; }
        }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags)
        {
            var cvDefn = store.Get(DfTags.CREATURE_VARIATION, tag.GetParam(0));
            var app = new DfCreatureVariationApplicator(cvDefn, tag.GetParams().ToArray());
            app.Apply(store, context);
        }
    }

    
}
