using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Combat
{
    public interface ICombatTargetDiscoverer
    {
        IEnumerable<IAgent> GetPossibleTargets(IAgent agent, IAtlas atlas);
    }

    public class CombatTargetDiscoverer : ICombatTargetDiscoverer
    {
        public IEnumerable<IAgent> GetPossibleTargets(IAgent agent, IAtlas atlas)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x != 0 || y != 0)
                    {
                        var tile = atlas.GetTileAtPos(agent.Pos + new Math.Vector3(x, y, 0));
                        if (tile.HasAgent && !tile.Agent.IsDead) yield return tile.Agent;
                    }
                }
            }
        }
    }
}
