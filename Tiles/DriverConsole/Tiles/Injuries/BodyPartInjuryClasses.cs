using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Injuries
{
    public static class BodyPartInjuryClasses
    {
        public static IBodyPartInjuryClass Severed = new BodyPartInjuryClass
        {
            //IsCompletion = true,
            IsSever = true,
            CompletionPhrase = "the severed part sails off in an arc"
        };

        public static IBodyPartInjuryClass ExplodesIntoGore = new BodyPartInjuryClass
        {
            //IsCompletion = true,
            CompletionPhrase = "the injured part explodes into gore"
        };

        public static IBodyPartInjuryClass ClovenAsunder = new BodyPartInjuryClass
        {
            //IsCompletion = true,
            CompletionPhrase = "the injured part is cloven asunder"
        };

        public static IBodyPartInjuryClass JustTissueDamage = new BodyPartInjuryClass
        {

        };
    }
}
