using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Random;

namespace Tiles.Agents.Behaviors
{
    public class AgentCommandQueue : IAgentCommandQueue
    {
        Queue<IAgentCommand> Queue { get; set; }
        public AgentCommandQueue()
        {
            Queue = new Queue<IAgentCommand>();
        }

        public void Enqueue(IAgentCommand command)
        {
            Queue.Enqueue(command);
        }

        public bool Any()
        {
            return Queue.Any();
        }

        public IAgentCommand Next()
        {
            return Queue.Dequeue();
        }

        public IAgentCommand Peek()
        {
            return Queue.Peek();
        }
    }
}
