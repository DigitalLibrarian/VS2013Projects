using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Bodies.Health.Injuries
{
    public interface IDamageResistor
    {
        bool Resist(IDamageVector damage);
    }
}
