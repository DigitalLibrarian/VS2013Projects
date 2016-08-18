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
    public class Verb : ConjugatedWord, IVerb
    {
        public bool IsTransitive { get; private set; }

        public Verb(IDictionary<VerbConjugation, string> conjugationMap, bool isTransitive)
            : base(conjugationMap)
        {
            IsTransitive = isTransitive;
        }
    }
}
