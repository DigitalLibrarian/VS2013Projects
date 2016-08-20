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

        public DfObjectContext(DfObject source, int cursor)
        {
            Source = source;
            Cursor = cursor;

            Working = Source.Tags.Select(x => x.CloneDfTag()).ToList();
        }

        public int RemoveTagsByName(string tagName)
        {
            int total = 0;
            foreach (var token in Working.Where(x => tagName == x.Name).ToList())
            {
                if (Cursor > Working.IndexOf(token)) Cursor--;
                Working.Remove(token);
                total++;
            }
            return total;
        }

        public void GoToStart()
        {
            if (GoToTag(Working.First().Name))
            {
                Cursor++;
            }
            else
            {
                Cursor = 0;
            }
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
            return new DfObject(Working);
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
    }
}
