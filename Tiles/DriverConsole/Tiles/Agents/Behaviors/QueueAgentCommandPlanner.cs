using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Random;

namespace Tiles.Agents.Behaviors
{
    public class QueueAgentCommandPlanner : BaseAgentCommandPlanner, IAgentCommandQueueProducer
    {
        Queue<IAgentCommand> Queue { get; set; }
        public QueueAgentCommandPlanner(IRandom random, IAgentCommandFactory commandFactory)
            : base(random, commandFactory, new AttackMoveDiscoverer(new AttackMoveBuilder(new DamageCalc())))
        {
            Queue = new Queue<IAgentCommand>();
        }

        void IAgentCommandQueueProducer.Enqueue(IAgentCommand command)
        {
            Queue.Enqueue(command);
        }

        bool IAgentCommandQueueProducer.Any()
        {
            return Queue.Any();
        }

        IAgentCommand Next(IAgent agent)
        {
            if (Queue.Any())
            {
                return Queue.Dequeue();
            }
            return Nothing(agent);
        }

        public override IAgentCommand PlanBehavior(IGame game, IAgent agent)
        {
            return Next(agent);
        }
    }
}
