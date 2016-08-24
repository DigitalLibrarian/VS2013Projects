using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfItemBuilder : IDfItemBuilder
    {
        Item Item { get; set; }

        public DfItemBuilder()
        {
            Item = new Item();
        }

        public void SetMaterial(Material material)
        {
            Item.Material = material;
        }

        public void AddCombatMove(CombatMove move)
        {
            AutovivWeapon();
            Item.Weapon.Moves.Add(move);
        }

        void AutovivWeapon()
        {
            if (Item.Weapon == null)
            {
                Item.Weapon = new Weapon();
            }
        }

        void AutovivArmor()
        {
            if (Item.Armor == null)
            {
                Item.Armor = new Armor();
            }
        }

        public void SetName(string singular, string plural)
        {
            Item.NameSingular = singular;
            Item.NamePlural = plural;
        }

        public void SetArmorLayer(string layer)
        {
            AutovivArmor();
            Item.Armor.ArmorLayer = layer;
        }

        public Item Build()
        {
            return Item;
        }


        public void AddSlotRequirement(ArmorSlot slot)
        {
            AutovivArmor();
            Item.Armor.SlotRequirements.Add(slot);
        }

        public void AddSlotRequirement(WeaponSlot slot)
        {
            AutovivWeapon();
            Item.Weapon.SlotRequirements.Add(slot);
        }
    }
}
