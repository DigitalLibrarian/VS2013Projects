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

        public Item(IItemClass itemClass)
        {
            Class = itemClass;
        }

        public bool IsWeapon { get { return Class.WeaponClass != null; } }
        public bool IsArmor { get { return Class.ArmorClass != null; } }


        public double GetMass()
        {
            //Armor size is calculated as underlying body part size times coverage/100%

            var sizeCm3 = Class.Size;
            return Class.Material.GetMassForUniformVolume(sizeCm3);
        }


    }
}
