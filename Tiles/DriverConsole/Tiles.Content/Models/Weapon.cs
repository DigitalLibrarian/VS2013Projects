using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Weapon
    {
        public List<string> SlotRequirements { get; set; }
        public List<CombatMove> Moves { get; set; }
    }
}
