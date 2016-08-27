using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Items;

namespace Tiles.Bodies.Health.Injuries
{
    public interface IDamageResistorFactory
    {
        IDamageResistor Create(IItem item);
        IDamageResistor Create(ITissueLayer layer);
    }
}
