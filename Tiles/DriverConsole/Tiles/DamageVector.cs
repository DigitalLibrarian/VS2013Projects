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
            else
            {
                Set(damageType, 0);
                return GetFraction(damageType);
            }
        }
        public void Set(DamageType damageType, int damage)
        {
            if (!Data.ContainsKey(damageType))
            {
                Data[damageType] = CreateDefaultFraction();
            }
            Data[damageType].Numerator = damage;
        }

        private Fraction CreateDefaultFraction()
        {
            return new Fraction(0, 100);
        }

        public IEnumerable<DamageType> GetTypes()
        {
            return Data.Keys.OrderByDescending(key => Data[key].AsDouble());
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

        public DamageType GetHighestDamageType()
        {
            return GetTypes().FirstOrDefault();
        }
    }
}
