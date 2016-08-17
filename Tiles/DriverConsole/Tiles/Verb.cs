using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Math;

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

    public class Verb : ConjugatedWord, IVerb
    {
        public bool IsTransitive { get; private set; }

        public Verb(IDictionary<VerbConjugation, string> conjugationMap, bool isTransitive)
            : base(conjugationMap)
        {
            IsTransitive = isTransitive;
        }
    }

    public class MissingVerbConjugationException : Exception
    {
        public VerbConjugation VerbConjugation { get; private set; }

        public MissingVerbConjugationException(VerbConjugation vc)
            : base(string.Format("Missing verb conjugation {0}.", vc))
        {
            VerbConjugation = vc;
        }
    }
}
