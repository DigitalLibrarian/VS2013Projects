using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Random;

namespace Tiles.Agents.Combat
{
    public class DamageCalc : IDamageCalc
    {
        public uint MeleeStrikeMoveDamage(IAgent attacker, IAgent defender, IBodyPart bodyPart, IWeapon weapon)
        {
            int total = 0;
            DamageTypes((damageType) =>
            {
                double dmg = (double)weapon.GetBaseTypeDamage(damageType);
                if (dmg > 0)
                {
                    if (bodyPart.Armor != null)
                    {
                        double resist = (double)bodyPart.Armor.GetTypeResistence(damageType);
                        dmg *= ((100d - resist) / 100d);
                    }
                    if (dmg > 0)
                    {
                        total += (int)dmg;
                    }
                }
            });
            return (uint)System.Math.Max(0, total);
        }

        public uint ThrownItemDamage(IAgent agent, IAgent defender, IBodyPart bodyPart, IItem item)
        {
            // TODO - if weapon, apply damage vector
            return HealthVector.MaxHealth;
        }

        static void DamageTypes(Action<DamageType> action)
        {
            foreach (var dt in DamageVector.AllDamageTypes())
            {
                action(dt);
            }
        }
    }
}
