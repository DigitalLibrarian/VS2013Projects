using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Items;

namespace Tiles.Bodies.Health.Injuries
{
    public class DamageResistorFactory : IDamageResistorFactory
    {
        public IDamageResistor Create(IItem item)
        {
            return new DamageResistor(item.Class.ArmorClass.ResistVector);
        }

        public IDamageResistor Create(ITissueLayer layer)
        {
            // tissues don't resist... for now
            return new DamageResistor(new DamageVector());
        }
    }
}
