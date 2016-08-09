using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;

namespace Tiles.Bodies
{
    public class BodyPart : IBodyPart
    {
        public string Name { get; private set; }
        public IBodyPart Parent { get; set; }
        public HealthVector Health { get; private set; }
        public ArmorSlot ArmorSlot { get; private set; }
        public IArmor Armor { get; set; }
        public WeaponSlot WeaponSlot { get; private set; }
        public IWeapon Weapon { get; set; }
        public bool IsCritical { get; private set; }
        public bool CanAmputate { get; private set; }

        public BodyPart(string name, bool isCritical, bool canAmputate, ArmorSlot armorSlotType, WeaponSlot weaponSlotType, IBodyPart parent)
        {
            Name = name;
            ArmorSlot = armorSlotType;
            WeaponSlot = weaponSlotType;
            Parent = parent;
            IsCritical = isCritical;
            CanAmputate = true;
            Health = HealthVector.Create();
        }

        public BodyPart(string name, bool isCritical, bool canAmputate, ArmorSlot armorSlotType, WeaponSlot weaponSlotType)
            : this(name, isCritical,canAmputate, armorSlotType, weaponSlotType, null)
        {

        }
    }
}
