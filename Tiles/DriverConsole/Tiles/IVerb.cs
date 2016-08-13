using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles
{
    public interface IVerb
    {
        bool IsTransitive { get; }
        string Conjugate(VerbConjugation con);
    }


    public interface INoun
    {
        bool IsProper { get; }
        string Conjugate(VerbConjugation conjugation);
    }


}
