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
            var allDamageTypes = Resist.GetComponentTypes()
                .Concat(damage.GetComponentTypes())
                .ToList();

            foreach(var dt in allDamageTypes)
            {
                var dComp = damage.GetComponent(dt);
                var rComp = Resist.GetComponent(dt);
                double scale = ((rComp) / 100d);
                if (scale > 0)
                {
                    var newComp = dComp - (uint)((dComp * scale));
                    damage.SetComponent(dt, newComp);
                }
            }

            return !damage.AnyPositive;
        }
    }
}
