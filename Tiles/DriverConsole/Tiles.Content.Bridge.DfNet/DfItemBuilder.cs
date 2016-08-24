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
            if (Item.Weapon == null)
            {
                Item.Weapon = new Weapon
                {
                    Moves = new List<CombatMove>()
                };
            }
            Item.Weapon.Moves.Add(move);
        }

        public void SetName(string singular, string plural)
        {
            Item.NameSingular = singular;
            Item.NamePlural = plural;
        }

        public Item Build()
        {
            return Item;
        }
    }
}
