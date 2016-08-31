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
        public static readonly int MiscItemSymbol = 157;
        Item Item { get; set; }

        int Symbol { get; set; }
        Color Foreground { get; set; }
        Color Background { get; set; }

        public DfItemBuilder()
        {
            Item = new Item();
            Symbol = MiscItemSymbol;
            Foreground = new Color(255, 255, 255, 255);
            Background = new Color(0, 0, 0, 255);
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
            Item.Sprite = new Sprite(Symbol, Foreground, Background);
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

        public void SetForegroundColor(Color color)
        {
            Foreground = color;
        }

        public void SetBackgroundColor(Color color)
        {
            Background = color;
        }

        public void SetSymbol(int symbol)
        {
            Symbol = symbol;
        }

        public void SetSize(int size)
        {
            Item.Size = size;
        }
    }
}
