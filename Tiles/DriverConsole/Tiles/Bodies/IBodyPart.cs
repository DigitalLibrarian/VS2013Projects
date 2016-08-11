using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Items;

namespace Tiles.Bodies
{
    public interface IBodyPart
    {
        string Name { get; }
        IBodyPart Parent { get; }
        bool IsLifeCritical { get; }
        bool CanAmputate { get; }
        HealthVector Health { get; }

        ArmorSlot ArmorSlot { get; }
        IItem Armor { get; set; }
        WeaponSlot WeaponSlot { get; }
        IItem Weapon { get; set; }
    }
}
