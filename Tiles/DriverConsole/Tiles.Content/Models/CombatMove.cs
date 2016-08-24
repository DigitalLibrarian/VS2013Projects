using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class CombatMove
    {
        public CombatMove()
        {
            Verb = new Verb();
        }
        public string Name { get; set; }
        public Verb Verb { get; set; }

        public BodyStateChange BodyStateChange { get; set; }

        public int PrepTime { get; set; }
        public int RecoveryTime { get; set; }

        public bool IsDefenderPartSpecific { get; set; }
        public bool IsMartialArts { get; set; }
        public bool IsStrike { get; set; }
        public bool IsItem { get; set; }

        public int ContactArea { get; set; }
        public int MaxPenetration { get; set; }
        public int VelocityMultiplier { get; set; }
    }
}
