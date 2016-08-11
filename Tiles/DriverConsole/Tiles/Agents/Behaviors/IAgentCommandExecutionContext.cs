using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Behaviors
{
    public class AgentCommandExecutionCompleteEventArgs : EventArgs
    {
        public long FrameTimeUsed { get; set; }
        public IAgent Agent { get; set; }
        public IAgentCommand Command { get; set; }
    }

    public interface IAgentCommandExecutionContext
    {
        bool HasCommand { get; }
        bool Executed { get; }
        long TimeRemaining { get; }

        event EventHandler CommandComplete;

        void StartNewCommand(IGame game, IAgentCommand command);
        long Execute(IGame game, IAgent agent, long maxTimeSlice);

        void Update(IGame game, IAgent agent);
    }
}
