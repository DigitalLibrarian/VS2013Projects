using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Math;

namespace Tiles.Items
{
    public interface IItem
    {/*
        string Name { get; }
        ISprite Sprite { get; }
      * */
        IItemClass Class { get; }
        bool IsWeapon { get; }
        //IWeaponClass WeaponClass { get; }

        bool IsArmor { get; }
        //IArmorClass ArmorClass { get; }
    }

    public interface IItemClass
    {
        string Name { get; }
        ISprite Sprite { get; }
        IWeaponClass WeaponClass { get; }
        IArmorClass ArmorClass { get; }
    }

    public class ItemClass : IItemClass
    {
        public string Name { get; set; }
        public ISprite Sprite { get; set; }
        public IWeaponClass WeaponClass { get; set; }
        public IArmorClass ArmorClass { get; set; }
    }
}
