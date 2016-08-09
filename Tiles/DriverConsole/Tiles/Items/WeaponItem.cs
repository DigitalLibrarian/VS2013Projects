using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Items
{
    public class Item : IItem
    {
        public string Name { get; set; }
        public ISprite Sprite { get; set; }
        public bool IsWeapon { get { return Weapon != null; } }
        public IWeapon Weapon { get; set; }
        public bool IsArmor { get { return Armor != null; } }
        public IArmor Armor { get; set; }
    }

    public class WeaponItem : IWeapon, IItem
    {
        public IWeaponClass WeaponClass { get; private set; }
        public WeaponItem(IWeaponClass weaponClass)
        {
            WeaponClass = weaponClass;
        }

        public bool IsWeapon { get { return true; } }
        public IWeapon Weapon { get { return this; } }

        public string Name { get { return WeaponClass.Name; } }
        public ISprite Sprite { get { return WeaponClass.Sprite; } }


        public bool IsArmor
        {
            get { return false; }
        }

        public IArmor Armor
        {
            get { return null; }
        }

        public IItem Item
        {
            get { return this; }
        }

        public uint GetBaseTypeDamage(DamageType damageType)
        {
            return WeaponClass.DamageVector.GetComponent(damageType);
        }
    }
}
