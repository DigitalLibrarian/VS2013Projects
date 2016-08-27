using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Bodies.Health.Injuries
{
    public interface IInjuryResultBuilder
    {
        void SetTargetBodyPart(IBodyPart bodyPart);

        void AddDamage(IDamageVector damage);
        void AddArmorResistor(IDamageResistor resistor);
        void AddTissueResistor(ITissueLayer layer, IDamageResistor resistor);

        IEnumerable<IInjury> Build();
    }
}
