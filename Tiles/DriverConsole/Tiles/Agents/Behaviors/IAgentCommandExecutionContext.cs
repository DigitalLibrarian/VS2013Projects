using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Behaviors
{
    public interface IAgentCommandExecutionContext
    {
        bool HasCommand { get; }
        bool Executed { get; }

        event EventHandler CommandComplete;

        void StartNewCommand(IGame game, IAgentCommand command);
        long Execute(IGame game, IAgent agent, long maxTimeSlice);

        long TimeRemaining { get; }
    }
}
