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
    public class AttackConductor : IAttackConductor
    {
        IList<ICombatEvolution> Evolutions { get; set; }

        public AttackConductor(IList<ICombatEvolution> evolutions)
        {
            Evolutions = evolutions;
        }

        public void Conduct(IAgent attacker, IAgent defender, ICombatMove move)
        {
            var context = new CombatMoveContext(attacker, defender, move);

            foreach (var evo in Evolutions)
            {
                evo.Evolve(context);
            }
        }
    }
}
