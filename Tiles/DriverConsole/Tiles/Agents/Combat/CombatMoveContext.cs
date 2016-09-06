using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies.Health.Injuries;

namespace Tiles.Agents.Combat
{
    public class CombatMoveContext : ICombatMoveContext
    {
        public ICombatMove Move { get; set; }

        public IAgent Attacker { get; set; }
        public IAgent Defender { get; set; }
        public List<IInjury> NewInjuries { get; set; }
    }
}
