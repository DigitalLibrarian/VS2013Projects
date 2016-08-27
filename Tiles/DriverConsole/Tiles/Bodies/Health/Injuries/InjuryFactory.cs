using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Health.Injuries
{
    public class InjuryFactory : IInjuryFactory
    {
        public IInjury Create(IInjuryClass injuryClass, IBodyPart bodyPart, ITissueLayer tissueLayer)
        {
            return new Injury(injuryClass, bodyPart, tissueLayer);
        }

        public IInjury Create(IInjuryClass injuryClass, IBodyPart bodyPart)
        {
            return new Injury(injuryClass, bodyPart);
        }
    }
}
