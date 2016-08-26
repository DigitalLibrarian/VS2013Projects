using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Random;

namespace Tiles.Agents.Behaviors
{
    public class DefaultAgentCommandPlanner : BaseAgentCommandPlanner
    {
        public DefaultAgentCommandPlanner(IRandom random, IAgentCommandFactory commandFactory, ICombatMoveDiscoverer moveDisco, IPositionFinder posFinder) 
            : base(random, commandFactory, moveDisco, posFinder) { }

        public override IEnumerable<IAgentCommand> PlanBehavior(IGame game, IAgent agent)
        {
            if (agent.IsDead) return Dead(agent);

            double wanderProb = 0.15;

            var target = FindTarget(game, agent);
            if (target != null)
            {
                var attackMoves = AttackMoves(agent, target);
                if (attackMoves.Any())
                {
                    var attackMove = Random.NextElement(attackMoves.ToList());
                    return CommandFactory.MeleeAttack(agent, target, attackMove);
                }
                else if (Random.NextDouble() > wanderProb)
                {
                    return Seek(agent, target.Pos);
                }
            }
            return Wander(agent);
        }

        // TODO - move into base class and test
        private IAgent FindTarget(IGame game, IAgent agent)
        {
            // TODO - the sight radius should depend on agent class data (skills, vision, etc..)
            var pos = FindNearbyPos(agent.Pos, worldPos =>
            {
                var tile = game.Atlas.GetTileAtPos(worldPos);
                return (tile.HasAgent && ((!tile.Agent.IsUndead && !tile.Agent.IsDead) || tile.Agent.IsPlayer));
            }, 8);

            if (pos.HasValue)
            {
                return game.Atlas.GetTileAtPos(pos.Value).Agent;
            }
            return null;
        }
    }
}
