using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Combat
{
    public interface IDodgeAgentCommandDiscoverer
    {
        IEnumerable<IAgentCommand> GetPossibleDodges(IAgent agent, IAgent other, IAtlas atlas);
    }

    public class DodgeAgentCommandDiscoverer : IDodgeAgentCommandDiscoverer
    {
        private static readonly AgentCommandType[] DodgeableCommandTypes = new AgentCommandType[]{
            AgentCommandType.AttackMeleePrep,
            AgentCommandType.AttackMelee
        };

        private IAgentCommandFactory CommandFactory { get; set; }
        public DodgeAgentCommandDiscoverer(IAgentCommandFactory commandFactory)
        {
            CommandFactory = commandFactory;
        }

        public IEnumerable<IAgentCommand> GetPossibleDodges(IAgent agent, IAgent other, IAtlas atlas)
        {
            if (other.AgentBehavior.Context.HasCommand
                && DodgeableCommandTypes.Contains(other.AgentBehavior.Context.Command.CommandType)
                && other.AgentBehavior.Context.Command.AttackMove != null)
            {
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            var offset = new Math.Vector3(x, y, 0);
                            var tile = atlas.GetTileAtPos(agent.Pos + offset);
                            if (!tile.HasAgent && tile.IsTerrainPassable)
                                foreach (var command in CommandFactory.DodgeDirection(agent, other, offset))
                                {
                                    yield return command;
                                }
                        }
                    }
                }
            }
        }
    }
}
