using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Math;

namespace Tiles
{
    public interface IDamageVector
    {
        IEnumerable<DamageType> GetTypes();

        int Get(DamageType damageType);
        void Set(DamageType damageType, long damage);

        Fraction GetFraction(DamageType damageType);

        void Add(IDamageVector damage);

        string ToString();
    }
}
