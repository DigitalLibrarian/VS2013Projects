using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies.Health.Injuries;

namespace Tiles.Agents.Combat
{
    public interface ICombatMoveContext
    {
        IAgent Attacker { get; }
        IAgent Defender { get; }
        ICombatMove Move { get; }

        List<IInjury> NewInjuries { get; }
    }
}
