using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Items
{
    public class Item : IItem
    {
        public IItemClass Class { get; set; }

        public bool IsWeapon { get { return Class.WeaponClass != null; } }
        public bool IsArmor { get { return Class.ArmorClass != null; } }
        /*
        public string Name { get; set; }
        public ISprite Sprite { get; set; }
        public IWeaponClass WeaponClass { get; set; }
        public IArmorClass ArmorClass { get; set; }
         * */
    }
}
