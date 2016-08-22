using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Parsing
{
    public class DfTagParser : IDfTagParser
    {
        private static char TagEnd = ']';
        private static char ParamSeparator = ':';
        private static char TagStart = '[' ;
        private static char[] ChunkSplit = new char[] { TagStart };
        
        public IEnumerable<DfTag> Parse(string line)
        {
            int tagStartIndex = line.IndexOf(TagStart);
            int tagEndIndex = line.IndexOf(TagEnd);
            if (tagStartIndex < tagEndIndex)
            {
                foreach (var chunk in line.Split(ChunkSplit))
                {
                    tagEndIndex = chunk.IndexOf(TagEnd);
                    if (tagEndIndex > 0)
                    {
                        var tagGuts = chunk.Substring(0, tagEndIndex);
                        yield return
                            new DfTag(tagGuts.
                                Split(ParamSeparator).ToArray());
                    }
                }
            }
        }
    }
}
