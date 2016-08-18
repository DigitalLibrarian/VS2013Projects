using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles
{
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
