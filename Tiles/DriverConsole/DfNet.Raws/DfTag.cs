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
        public DfTag(string name, params string[] parameters)
            : this(new List<string>{name}.Concat(parameters).ToArray()){

        }
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

        public string[] GetParams()
        {
            var p = new List<string>();
            for (int i = 0; i < NumWords - 1; i++)
            {
                p.Add(GetParam(i));
            }
            return p.ToArray();
        }

        public string[] GetWords()
        {
            var w = new List<string>();
            for (int i = 0; i < NumWords; i++)
            {
                w.Add(GetWord(i));
            }
            return w.ToArray();
        }

        public string GetParam(int i)
        {
            return GetWord(i + 1);
        }

        public DfTag CloneDfTag()
        {
            return new DfTag(Words.Select(x => x.Clone() as string).ToArray());
        }

        public DfTag CloneWithArgs(string argPrefix, string[] args)
        {
            return new DfTag(Name, ApplyArgs(argPrefix, args, GetParams()));
        }

        string[] ApplyArgs(string argPrefix, string[] args, string[] p)
        {
            var newParams = new List<string>();
            foreach (var pIn in p)
            {
                if (pIn.StartsWith(argPrefix) )
                {
                    int index = int.Parse(pIn.Substring(argPrefix.Length))-1;
                    newParams.Add(args[index]);
                }
                else
                {
                    newParams.Add(pIn);
                }
            }

            return newParams.ToArray();
        }

        public bool IsSingleWord(string name)
        {
            return IsSingleWord() && Name.Equals(name);
        }

        public bool IsSingleWord()
        { 
            return NumWords == 1; 
        }


        public override string ToString()
        {
            return string.Format("[{0}]", string.Join(":", Words));
        }

        public class NoWordsException : Exception
        {
            public NoWordsException() : base() { }
        }

        public int NumParams { get { return NumWords - 1; } }
    }
}
