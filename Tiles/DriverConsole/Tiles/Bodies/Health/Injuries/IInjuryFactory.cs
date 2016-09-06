using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Bodies.Health.Injuries
{
    public interface IInjuryFactory
    {
        // TODO - these might probably operate off part and tissue layer classes
        // rather than the instances themselves

        IInjury Create(IInjuryClass injuryClass, IBodyPart bodyPart);
        IInjury Create(IInjuryClass injuryClass, IBodyPart bodyPart, IDamageVector damage);
    }


}
