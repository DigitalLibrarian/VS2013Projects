using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Combat
{
    public class DamageVector : IDamageVector
    {
        IDictionary<DamageType, uint> Data { get; set; }

        public bool AnyPositive
        {
            get { return Data.Values.Any(v => v > 0) && Data.Values.Any(); }
        }

        public DamageVector(IDictionary<DamageType, uint> data)
        {
            Data = data;
        }

        public DamageVector()
        {
            Data = new Dictionary<DamageType, uint>();
        }

        public uint GetComponent(DamageType damageType)
        {
            if (Data.ContainsKey(damageType))
            {
                return Data[damageType];
            }
            else
            {
                return 0;
            }
        }

        public void SetComponent(DamageType damageType, uint damage)
        {
            Data[damageType] = damage;
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
