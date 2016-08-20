﻿using System;
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

        public DfObject(IList<DfTag> tags)
        {
            if(!tags.Any()) throw new InvalidHeaderException();
            if (tags.First().NumWords < 2) throw new InvalidHeaderException();
            Tags = tags;
        }

        public class InvalidHeaderException : Exception { }
    }
}
