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
        uint GetComponent(DamageType damageType);
        void SetComponent(DamageType damageType, uint damage);

        string ToString();
    }
}
