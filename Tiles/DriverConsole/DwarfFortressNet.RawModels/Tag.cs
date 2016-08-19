using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    public class Tag
    {
        public string Name { get { return Words.First(); } }
        public IList<string> Words { get; set; }

        public bool IsSingleWord(string p)
        {
             return IsSingleWord() && Words.Single().Equals(p);
        }

        public bool IsSingleWord()
        {
            return Words.Count() == 1;
        }

        internal Tag Clone()
        {
            return new Tag
            {
                Words = Words.ToList()
            };
        }
    }
}
