using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Armor
    {
        public Armor()
        {
            SlotRequirements = new List<ArmorSlot>();
        }
        public List<ArmorSlot> SlotRequirements { get; set; }

        public string ArmorLayer { get; set; }
    }
}
