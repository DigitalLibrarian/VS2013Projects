using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
