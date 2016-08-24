using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Weapon
    {
        List<string> SlotRequirements { get; set; }
        List<CombatMove> Moves { get; set; }
    }
}
