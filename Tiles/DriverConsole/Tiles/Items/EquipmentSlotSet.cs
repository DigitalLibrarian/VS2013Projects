using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Items
{
    public class EquipmentSlotSet : IEquipmentSlotSet
    {
        IDictionary<WeaponSlot, IBodyPart> Weapons { get; set; }
        IDictionary<ArmorSlot, IBodyPart> Armors { get; set; }

        IBody Body { get; set; }

        public EquipmentSlotSet(IBody body)
        {
            Body = body;
            Weapons = new Dictionary<WeaponSlot, IBodyPart>();
            Armors = new Dictionary<ArmorSlot, IBodyPart>();
            foreach (var part in Body.Parts)
            {
                if (part.WeaponSlot != WeaponSlot.None)
                {
                    Weapons[part.WeaponSlot] = part;
                }

                if (part.ArmorSlot != ArmorSlot.None)
                {
                    Armors[part.ArmorSlot] = part;
                }
            }
        }

        bool BodyHasPart(IBodyPart part)
        {
            return Body.Parts.Contains(part);
        }

        public bool HasSlot(WeaponSlot slot)
        {
            if (Weapons.ContainsKey(slot))
            {
                var part = Weapons[slot];
                return BodyHasPart(part);
            }
            return false;
        }

        public bool IsSlotFull(WeaponSlot slot)
        {
            if (Weapons.ContainsKey(slot))
            {
                var part = Weapons[slot];
                return BodyHasPart(part) && part.Weapon != null;
            }
            return false;
        }

        public IWeapon Get(WeaponSlot slot)
        {
            if (Weapons.ContainsKey(slot))
            {
                var part = Weapons[slot];
                if (BodyHasPart(part)) return part.Weapon;
            }
            return null;
        }

        public IWeapon Empty(WeaponSlot slot)
        {
            var part = Weapons[slot];
            if (BodyHasPart(part))
            {
                var weapon = part.Weapon;
                part.Weapon = null;
                return weapon;
            }
            return null;
        }

        public void Fill(WeaponSlot slot, IWeapon weapon)
        {
            var part = Weapons[slot];
            if (BodyHasPart(part) && part.Weapon == null)
            {
                part.Weapon = weapon;
            }
        }


        public bool HasSlot(ArmorSlot slot)
        {
            if (Armors.ContainsKey(slot))
            {
                var part = Armors[slot];
                return BodyHasPart(part);
            }
            return false;
        }

        public bool IsSlotFull(ArmorSlot slot)
        {
            if (Armors.ContainsKey(slot))
            {
                var part = Armors[slot];
                return BodyHasPart(part) && part.Armor != null;
            }
            return false;
        }

        public IArmor Get(ArmorSlot slot)
        {
            if (Armors.ContainsKey(slot))
            {
                var part = Armors[slot];
                if (BodyHasPart(part)) return part.Armor;
            }
            return null;
        }


        public IArmor Empty(ArmorSlot slot)
        {
            var part = Armors[slot];
            if (BodyHasPart(part))
            {
                var armor = part.Armor;
                part.Armor = null;
                return armor;
            }
            return null;
        }

        public void Fill(ArmorSlot slot, IArmor armor)
        {
            var part = Armors[slot];
            if (BodyHasPart(part) && part.Armor == null)
            {
                part.Armor = armor;
            }
        }

        public IWeapon GetWeapon(IBodyPart part)
        {
            if (!BodyHasPart(part)) return null;
            return Get(part.WeaponSlot);
        }

        public IArmor GetArmor(IBodyPart part)
        {
            if (!BodyHasPart(part)) return null;
            return Get(part.ArmorSlot);
        }



        public IEnumerable<WeaponSlot> AvailableWeaponSlots()
        {
            foreach (var part in Body.Parts)
            {
                if (part.WeaponSlot != WeaponSlot.None)
                {
                    if (!IsSlotFull(part.WeaponSlot))
                    {
                        yield return part.WeaponSlot;
                    }
                }
            }
        }

        public IEnumerable<ArmorSlot> AvailableArmorSlots()
        {
            foreach (var part in Body.Parts)
            {
                if (part.ArmorSlot != ArmorSlot.None)
                {
                    if (!IsSlotFull(part.ArmorSlot))
                    {
                        yield return part.ArmorSlot;
                    }
                }
            }
        }
    }
}
