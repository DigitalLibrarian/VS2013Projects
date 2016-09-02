using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles
{
    public class DamageVector : IDamageVector
    {
        Dictionary<DamageType, Fraction> Data { get; set; }
        
        public DamageVector(IDictionary<DamageType, int> data)
        {
            Data = new Dictionary<DamageType, Fraction>();
            foreach (var pair in data)
            {
                Data.Add(pair.Key, new Fraction(pair.Value, 100));
            }
        }

        public DamageVector()
        {
            Data = new Dictionary<DamageType, Fraction>();
        }

        public int GetComponent(DamageType damageType)
        {
            if (Data.ContainsKey(damageType))
            {
                return (int) Data[damageType].Numerator;
            }
            else
            {
                return 0;
            }
        }

        public void SetComponent(DamageType damageType, int damage)
        {
            if (!Data.ContainsKey(damageType))
            {
                Data[damageType] = new Fraction(100, 100);
            }
            Data[damageType].Numerator = damage;
        }

        public IEnumerable<DamageType> GetComponentTypes()
        {
            return Data.Keys;
        }

        StringBuilder StringBuilder = new StringBuilder();
        public override string ToString()
        {
            StringBuilder.Clear();
            foreach (var dt in GetComponentTypes())
            {
                StringBuilder.AppendFormat("{0}: {1} ", dt, GetComponent(dt));
            }
            return StringBuilder.ToString();
        }

        public static IEnumerable<DamageType> AllDamageTypes()
        {
            foreach (var dtObj in Enum.GetValues(typeof(DamageType)))
            {
                yield return (DamageType)dtObj;
            }
        }


        public void Add(IDamageVector damage)
        {
            foreach (var dt in damage.GetComponentTypes())
            {
                SetComponent(dt, GetComponent(dt) + damage.GetComponent(dt));
            }
        }
    }
}
