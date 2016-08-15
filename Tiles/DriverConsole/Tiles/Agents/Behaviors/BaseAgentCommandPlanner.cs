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
        public BaseAgentCommandPlanner(IRandom random, IAgentCommandFactory commandFactory, ICombatMoveDiscoverer moveDisco)
        {
            Random = random;
            CommandFactory = commandFactory;
            AttackMoveDisco = moveDisco;
        }

        protected IAgentCommand Wander(IAgent agent)
        {
            var wander = Random.NextElement(CompassVectors.GetAll().Select(x => new Vector3(x.X, x.Y, 0)).ToList());
            return CommandFactory.MoveDirection(agent, wander);
        }

        protected IAgentCommand Nothing(IAgent agent)
        {
            return CommandFactory.Nothing(agent);
        }

        protected IAgentCommand Dead(IAgent agent)
        {
            return Nothing(agent);
        }

        protected IAgentCommand Seek(IAgent agent, Vector3 pos3d)
        {
            // ZHACK
            var agentPos = new Vector2(agent.Pos.X, agent.Pos.Y);
            var pos = new Vector2(pos3d.X, pos3d.Y);
            var diffVector =  pos - agentPos;
            var choices = new Dictionary<Vector2, double>();
            foreach (var cDir in CompassVectors.GetAll())
            {
                var dot = Vector2.Dot(diffVector, cDir);
                choices.Add(cDir, dot);
            }

            // Now we try them from smallest deviance angle to highest deviance angle
            var moveOrder = (from pair in choices
                             orderby pair.Value descending
                             select pair.Key).ToList();
            while (moveOrder.Any())
            {
                var move2d = moveOrder.ElementAt(0);
                moveOrder.RemoveAt(0);
                var move = new Vector3(move2d.X, move2d.Y, 0);

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
            for (int i = 0; i <= halfBoxSize; i++)
            {
                var centerWorldPos = new Vector2(
                    centerWorldPos3d.X, centerWorldPos3d.Y
                    );
                var halfSize = new Vector2(i, i);
                var box = new Box2(centerWorldPos - halfSize, centerWorldPos + halfSize);
                for (int x = box.Min.X; x <= box.Max.X; x++)
                {
                    for (int y = box.Min.Y; y <= box.Max.Y; y++)
                    {
                        if (x == box.Min.X || x == box.Max.X || y == box.Min.Y || y == box.Max.Y)
                        {
                            var worldPos = new Vector3(x, y, centerWorldPos3d.Z);
                            if (finderPred(worldPos))
                            {
                                return worldPos;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public abstract IAgentCommand PlanBehavior(IGame game, IAgent agent);
    }
}
