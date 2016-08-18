using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Behaviors
{
    public interface IAgentCommandQueue
    {
        void Enqueue(IAgentCommand command);
        IAgentCommand Next();

        bool Any();
    }
}
