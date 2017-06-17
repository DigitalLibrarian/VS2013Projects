using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Agents.Combat
{
    public interface IAttackConductor
    {
        void Conduct(IAgent attacker, IAgent defender, ICombatMove move);

        // event source for action logging
    }
}
