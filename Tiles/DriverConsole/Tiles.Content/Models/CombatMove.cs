using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class CombatMove
    {
        string Name { get; set; }

        int PrepTime { get; set; }
        int RecoveryTime { get; set; }

        bool IsDefenderPartSpecific { get; set; }
        bool IsMartialArts { get; set; }
        bool IsStrike { get; set; }
        bool IsItem { get; set; }

        int ContactArea { get; set; }
        int MaxPenetration { get; set; }
        int VelocityMultiplier { get; set; }
    }
}
