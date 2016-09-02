using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles
{
    public interface IDamageVector
    {
        IEnumerable<DamageType> GetComponentTypes();
        int GetComponent(DamageType damageType);
        void SetComponent(DamageType damageType, int damage);

        string ToString();

        void Add(IDamageVector damage);
    }
}
