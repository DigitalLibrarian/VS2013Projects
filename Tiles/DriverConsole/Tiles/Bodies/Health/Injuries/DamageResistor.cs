using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Bodies.Health.Injuries
{
    public class DamageResistor : IDamageResistor
    {
        IDamageVector Resist { get; set; }
        public DamageResistor(IDamageVector resist)
        {
            Resist = resist;
        }

        bool IDamageResistor.Resist(IDamageVector damage)
        {
            return false;
        }
    }
}
