using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Health.Injuries
{
    public class InjuryFactory : IInjuryFactory
    {
        public IInjury Create(IInjuryClass injuryClass, IBodyPart bodyPart, IDamageVector damage)
        {
            return new Injury(injuryClass, bodyPart, damage);
        }

        public IInjury Create(IInjuryClass injuryClass, IBodyPart bodyPart)
        {
            return Create(injuryClass, bodyPart, new DamageVector());
        }
    }
}
