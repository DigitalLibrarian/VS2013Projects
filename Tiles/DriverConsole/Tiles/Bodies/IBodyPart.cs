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
        bool IsCritical { get; }
        bool CanAmputate { get; }
        HealthVector Health { get; }

        ArmorSlot ArmorSlot { get; }
        IArmor Armor { get; set; }
        WeaponSlot WeaponSlot { get; }
        IWeapon Weapon { get; set; }
    }
}
