using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Math;
using Tiles.Random;

namespace Tiles.Agents.Behaviors
{
    public abstract class BaseAgentCommandPlanner : IAgentCommandPlanner
    {
        protected IAgentCommandFactory CommandFactory { get; private set; }
        ICombatMoveDiscoverer AttackMoveDisco { get; set; }
        protected IRandom Random { get; private set; }
        IPositionFinder PositionFinder { get; set; }
        public BaseAgentCommandPlanner(IRandom random, IAgentCommandFactory commandFactory, ICombatMoveDiscoverer moveDisco, IPositionFinder posFinder)
        {
            Random = random;
            CommandFactory = commandFactory;
            AttackMoveDisco = moveDisco;
            PositionFinder = posFinder;
        }

        protected IEnumerable<IAgentCommand> Wander(IAgent agent)
        {
            var wander = Random.NextElement(CompassVectors.GetAll().Select(x => new Vector3(x.X, x.Y, 0)).ToList());
            return CommandFactory.MoveDirection(agent, wander);
        }

        protected IEnumerable<IAgentCommand> Nothing(IAgent agent)
        {
            return CommandFactory.Nothing(agent);
        }

        protected IEnumerable<IAgentCommand> Dead(IAgent agent)
        {
            return Nothing(agent);
        }

        protected IEnumerable<IAgentCommand> Seek(IAgent agent, Vector3 pos)
        {
            // ZHACK
            var diffVector =  pos - agent.Pos;
            var choices = new Dictionary<Vector3, double>();
            foreach (var cDir in CompassVectors.GetAll())
            {
                var dot = Vector3.Dot(diffVector, cDir);
                choices.Add(cDir, dot);
            }

            // Now we try them from smallest deviance angle to highest deviance angle
            var moveOrder = (from pair in choices
                             orderby pair.Value descending
                             select pair.Key).ToList();
            while (moveOrder.Any())
            {
                var move = moveOrder.ElementAt(0);
                moveOrder.RemoveAt(0);

                if (agent.CanMove(move))
                {
                    return CommandFactory.MoveDirection(agent, move);
                }
            }
            return Wander(agent);
        }

        protected IEnumerable<ICombatMove> AttackMoves(IAgent agent, IAgent target)
        {
            return AttackMoveDisco.GetPossibleMoves(agent, target);
        }

        protected Vector3? FindNearbyPos(Vector3 centerWorldPos3d, Predicate<Vector3> finderPred, int halfBoxSize)
        {
            return PositionFinder.FindNearbyPos(centerWorldPos3d, finderPred, halfBoxSize);
        }

        public abstract IEnumerable<IAgentCommand> PlanBehavior(IGame game, IAgent agent);
    }
}
