using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Agents.Combat
{
    public interface IDamageVector
    {
        bool AnyPositive { get; }

        IEnumerable<DamageType> GetComponentTypes();
        uint GetComponent(DamageType damageType);
        void SetComponent(DamageType damageType, uint damage);

        string ToString();

        void Add(IDamageVector damage);
    }
}
