using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_CopyTagsFrom : IDfTagInterpreter
    {
        public string TagName { get { return DfTags.MiscTags.COPY_TAGS_FROM; } }

        public void Run(IDfObjectStore store, IDfObjectContext context, DfTag triggerTag, IList<DfTag> tags)
        {
            var creatureType = triggerTag.GetParam(0);
            var creatureDf = store.Get(DfTags.CREATURE, creatureType);

            context.InsertTags(creatureDf.Tags.Skip(1).ToArray());

            //context.CopyTagsFrom(creatureDf);
        }
    }
}
