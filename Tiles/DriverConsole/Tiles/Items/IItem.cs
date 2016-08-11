using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Math;

namespace Tiles.Items
{
    public interface IItem
    {
        string Name { get; }
        ISprite Sprite { get; }

        bool IsWeapon { get; }
        IWeaponClass WeaponClass { get; }

        bool IsArmor { get; }
        IArmorClass ArmorClass { get; }
    }
}
