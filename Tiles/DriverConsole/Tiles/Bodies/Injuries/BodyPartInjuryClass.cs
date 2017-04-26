using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Injuries
{
    public interface IBodyPartInjuryClass
    {
        bool IsCompletion { get; }
        string CompletionPhrase { get; }
        bool IsSever { get; }
    }

    public class BodyPartInjuryClass : IBodyPartInjuryClass
    {
        public bool IsCompletion { get; set; }
        public string CompletionPhrase { get; set; }
        public bool IsSever { get; set; }
    }
}
