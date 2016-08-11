using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles
{
    public class Verb : IVerb
    {
        Dictionary<VerbConjugation, string> Conjugations { get; set; }

        public Verb(string firstPerson, string secondPerson, string thirdPerson)
        {
            Conjugations = new Dictionary<VerbConjugation, string>
            {
                { VerbConjugation.FirstPerson , firstPerson},
                { VerbConjugation.SecondPerson , secondPerson },
                { VerbConjugation.ThirdPerson , thirdPerson }
            };
        }

        public string Conjugate(VerbConjugation vc)
        {
            if (Conjugations.ContainsKey(vc))
            {
                return Conjugations[vc];
            }
            throw new MissingVerbConjugationException(this, vc);
        }
    }

    public class MissingVerbConjugationException : Exception
    {
        public IVerb Verb { get; private set; }
        public VerbConjugation VerbConjugation { get; private set; }

        public MissingVerbConjugationException(IVerb verb, VerbConjugation vc)
            : base(string.Format("Missing verb conjugation {0} for {1}", vc, verb))
        {
            Verb = verb;
            VerbConjugation = vc;
        }
    }

}
