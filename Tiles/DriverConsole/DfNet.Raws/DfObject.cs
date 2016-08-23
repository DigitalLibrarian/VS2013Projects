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
            if(!tags.Any()) throw new InvalidHeaderException(tags);
            if (tags.First().NumWords < 2) throw new InvalidHeaderException(tags);
            Tags = tags;
        }

        public class InvalidHeaderException : Exception 
        {
            public InvalidHeaderException(IEnumerable<DfTag> tags) :
                base(string.Format("Invalid object header: {0}", string.Join("", tags.Select(t => t.ToString())))) { }
        }

        public DfObject CloneDfObject()
        {
            return new DfObject(Tags.Select(x => x.CloneDfTag()).ToList());
        }

        public DfObject CloneDfObjectWithArgs(string argPrefix, string[] args)
        {
            return new DfObject(Tags.Select(x => x.CloneWithArgs(argPrefix, args)).ToList());
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
