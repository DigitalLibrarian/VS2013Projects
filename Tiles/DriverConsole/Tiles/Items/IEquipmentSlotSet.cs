using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Items
{
    public interface IEquipmentSlotSet
    {
        bool HasSlot(WeaponSlot slot);
        bool HasSlot(ArmorSlot slot);

        bool IsSlotFull(WeaponSlot slot);
        bool IsSlotFull(ArmorSlot slot);

        IWeapon Get(WeaponSlot slot);
        IArmor Get(ArmorSlot slot);

        IWeapon Empty(WeaponSlot slot);
        IArmor Empty(ArmorSlot slot);

        void Fill(WeaponSlot slot, IWeapon weapon);
        void Fill(ArmorSlot slot, IArmor armor);

        IEnumerable<WeaponSlot> AvailableWeaponSlots();
        IEnumerable<ArmorSlot> AvailableArmorSlots();
    }
}
