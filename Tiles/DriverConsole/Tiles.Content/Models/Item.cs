using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Item
    {
        public string NameSingular { get; set; }
        public string NamePlural { get; set; }

        public Weapon Weapon { get; set; }
        public Armor Armor { get; set; }

        public Material Material { get; set; }

        public int Symbol { get; set; }
    }
}
