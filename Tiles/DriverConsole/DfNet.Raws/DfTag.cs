using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws
{
    public class DfTag
    {
        string[] Words { get; set; }
        public DfTag(params string[] words)
        {
            if (!words.Any()) throw new NoWordsException();
            Words = words;
        }

        public int NumWords { get { return Words.Count(); } }
        public string Name { get { return Words[0]; } }


        public string GetWord(int i)
        {
            return Words[i];
        }

        public string GetParam(int i)
        {
            return GetWord(i + 1);
        }

        public DfTag CloneTag()
        {
            return new DfTag(Words.Select(x => x.Clone() as string).ToArray());
        }
        
        public class NoWordsException : Exception
        {
            public NoWordsException() : base() { }
        }

        public bool IsSingleWord(string name)
        {
            return IsSingleWord() && Name.Equals(name);
        }

        public bool IsSingleWord()
        { 
            return NumWords == 1; 
        } 
    }
}
