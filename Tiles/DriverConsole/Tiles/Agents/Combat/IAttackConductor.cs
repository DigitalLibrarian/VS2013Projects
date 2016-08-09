using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;

namespace Tiles.Agents.Combat
{
    public interface IAttackConductor
    {
        void Conduct(IAgent attacker, IAgent defender, IAttackMove move);
    }
}
