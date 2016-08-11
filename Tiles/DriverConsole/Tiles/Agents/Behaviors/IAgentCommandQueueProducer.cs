using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Behaviors
{
    public interface IAgentCommandQueueProducer
    {
        void Enqueue(IAgentCommand command);

        bool Any();
    }
}
