using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies.Injuries;

namespace Tiles.Agents.Combat
{
    public class CombatMoveContext : ICombatMoveContext
    {
        public ICombatMove Move { get; set; }

        public IAgent Attacker { get; set; }
        public IAgent Defender { get; set; }
        public IInjuryReport InjuryReport { get; set; }

        public CombatMoveContext(IAgent attacker, IAgent defender, ICombatMove move)
        {
            Attacker = attacker;
            Defender = defender;
            Move = move;
        }
    }
}
