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

    public class DamageResistorFactory : IDamageResistorFactory
    {
        public IDamageResistor Create(IItem item)
        {
            return new DamageResisitor(item.Class.ArmorClass.ResistVector);
        }

        public IDamageResistor Create(ITissueLayer layer)
        {
            // tissues don't resist... for now
            return new DamageResisitor(new DamageVector());
        }
    }

    public class DamageResisitor : IDamageResistor
    {
        IDamageVector Resist { get; set; }
        public DamageResisitor(IDamageVector resist)
        {
            Resist = resist;
        }

        bool IDamageResistor.Resist(IDamageVector damage)
        {
            return false;
        }
    }
}
