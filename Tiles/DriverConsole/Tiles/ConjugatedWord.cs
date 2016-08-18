using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles
{
    public abstract class ConjugatedWord
    {
        IDictionary<VerbConjugation, string> Conjugations { get; set; }

        public ConjugatedWord(IDictionary<VerbConjugation, string> conjugations)
        {
            Conjugations = conjugations;
        }

        public string Conjugate(VerbConjugation vc)
        {
            if (Conjugations.ContainsKey(vc))
            {
                return Conjugations[vc];
            }
            throw new MissingVerbConjugationException(vc);
        }
    }
}
