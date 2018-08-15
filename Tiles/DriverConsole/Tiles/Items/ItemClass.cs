using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Items
{
    public class ItemClass : IItemClass
    {
        public string Name { get; set; }
        public Sprite Sprite { get; set; }
        public IWeaponClass WeaponClass { get; set; }
        public IArmorClass ArmorClass { get; set; }

        public IMaterial Material { get; set; }
        public double Size { get; set; }

        public ItemClass(string name, Sprite sprite, double size, IMaterial material, IWeaponClass weaponClass)
            : this(name, sprite, size, material, weaponClass, null) { }

        public ItemClass(string name, Sprite sprite, double size, IMaterial material, IArmorClass armorClass)
            : this(name, sprite, size, material, null, armorClass) { }

        public ItemClass(string name, Sprite sprite, double size, IMaterial material, IWeaponClass weaponClass, IArmorClass armorClass)
        {
            Name = name;
            Sprite = sprite;
            WeaponClass = weaponClass;
            ArmorClass = armorClass;
            Material = material;
            Size = size;
        }
    }
}
