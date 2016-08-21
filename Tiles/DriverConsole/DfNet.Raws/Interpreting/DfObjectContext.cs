using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting
{
    public class DfObjectContext : IDfObjectContext
    {
        public int Cursor { get; private set; }
        public DfObject Source { get; private set; }

        public List<DfTag> Working { get; private set; }

        public IEnumerable<DfTag> WorkingSet { get { return Working.ToArray(); } }

        public DfObjectContext(DfObject source, int cursor = 0)
        {
            Source = source;
            Cursor = cursor;

            Working = new List<DfTag>();

        }

        public void StartPass()
        {
            Working = new List<DfTag>();
            Cursor = 0;
        }
        public void EndPass()
        {
            Source = new DfObject(Working);
            Cursor = 0;
        }

        public int RemoveTagsByName(string tagName)
        {
            int total = 0;
            foreach (var token in Working.Where(x => tagName == x.Name).ToList())
            {
                if (Cursor >= Working.IndexOf(token)) Cursor--;
                Working.Remove(token);
                total++;
            }
            return total;
        }

        public void GoToStart()
        {
            Cursor = 1;
        }

        public void GoToEnd()
        {
            Cursor = Math.Max(Working.Count(), 0);
        }

        public bool GoToTag(string name)
        {
            var newIndex =  Working.FindIndex(tag => tag.Name.Equals(name));
            if (newIndex != -1)
            {
                Cursor = newIndex;
                return true;
            }
            return false;
        }
                
        public void InsertTags(params DfTag[] tags)
        {
            foreach (var tag in tags)
            {
                Working.Insert(Cursor++, tag);
            }
        }

        public DfObject Create()
        {
            return Source.CloneDfObject();
        }


        public bool ReplaceTag(DfTag original, DfTag newTag)
        {
            int index = Working.IndexOf(original);
            if (index != -1)
            {
                Working.Remove(original);
                Working.Insert(index, newTag);
                return true;
            }
            return false;
        }


        public void CopyTagsFrom(DfObject creatureDf)
        {
            var tags = creatureDf.Tags.Where(tag => !tag.Name.Equals(DfTags.CREATURE))
                .Select(t => t.CloneDfTag())
                .Where(t => !t.Name.Equals(DfTags.MiscTags.APPLY_CREATURE_VARIATION));

            InsertTags(tags.ToArray());
        }


        public void Remove(params DfTag[] dfTags)
        {
            foreach (var token in dfTags)
            {
                if (Cursor >= Working.IndexOf(token)) Cursor--;
                Working.Remove(token);
            }
        }
    }
}
