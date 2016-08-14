using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Behaviors;
using Tiles.Agents.Combat;
using Tiles.Random;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class ZombieAgentCommandPlanner : BaseAgentCommandPlanner
    {
        public ZombieAgentCommandPlanner(IRandom random, IAgentCommandFactory commandFactory) 
            : base(random, commandFactory, new CombatMoveDiscoverer(new CombatMoveBuilder(new DamageCalc()))) { }

        public override IAgentCommand PlanBehavior(IGame game, IAgent agent)
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
            var pos = FindNearbyPos(agent.Pos, worldPos =>
            {
                var tile = game.Atlas.GetTileAtPos(worldPos);
                return (tile.HasAgent && !tile.Agent.IsUndead && !tile.Agent.IsDead);
            }, 8);

            if (pos.HasValue)
            {
                return game.Atlas.GetTileAtPos(pos.Value).Agent;
            }
            return null;
        }
    }
}
