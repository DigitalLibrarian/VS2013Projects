using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws
{
    public class DfObject
    {
        public string Type { get { return Tags.First().GetWord(0); } }
        public string Name { get { return Tags.First().GetWord(1); } }
        public IList<DfTag> Tags { get; private set; }

        public DfObject(params DfTag[] tags) : this(new List<DfTag>(tags)) { }
        public DfObject(IList<DfTag> tags)
        {
            if(!tags.Any()) throw new InvalidHeaderException();
            if (tags.First().NumWords < 2) throw new InvalidHeaderException();
            Tags = tags;
        }

        public class InvalidHeaderException : Exception { }

        public DfObject CloneDfObject()
        {
            return new DfObject(Tags.Select(x => x.CloneDfTag()).ToList());
        }

        public DfTag Next(DfTag t)
        {
            if (Tags.Contains(t) && Tags.Last() != t)
            {
                var index = Tags.IndexOf(t);
                return Tags[index + 1];
            }
            return null;
        }
    }
}
