using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Items
{
    public interface IArmor
    {
        IArmorClass ArmorClass { get; }

        uint GetTypeResistence(DamageType damageType);
    }

}
