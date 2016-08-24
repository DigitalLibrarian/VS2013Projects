using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Weapon
    {
        public Weapon()
        {
            SlotRequirements = new List<WeaponSlot>();
            Moves = new List<CombatMove>();
        }

        public List<WeaponSlot> SlotRequirements { get; set; }
        public List<CombatMove> Moves { get; set; }
    }
}
