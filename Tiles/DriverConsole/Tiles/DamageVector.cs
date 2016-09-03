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

        public int Get(DamageType damageType)
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

        public Fraction GetFraction(DamageType damageType)
        {
            if (Data.ContainsKey(damageType))
            {
                return Data[damageType];
            }
            return null;
        }
        public void Set(DamageType damageType, int damage)
        {
            if (!Data.ContainsKey(damageType))
            {
                Data[damageType] = new Fraction(100, 100);
            }
            Data[damageType].Numerator = damage;
        }

        public IEnumerable<DamageType> GetTypes()
        {
            return Data.Keys;
        }

        StringBuilder StringBuilder = new StringBuilder();
        public override string ToString()
        {
            StringBuilder.Clear();
            foreach (var dt in GetTypes())
            {
                StringBuilder.AppendFormat("{0}: {1} ", dt, Get(dt));
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
            foreach (var dt in damage.GetTypes())
            {
                Set(dt, Get(dt) + damage.Get(dt));
            }
        }
    }
}
