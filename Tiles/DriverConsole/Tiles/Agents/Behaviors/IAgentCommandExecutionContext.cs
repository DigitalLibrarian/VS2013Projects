using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Behaviors
{
    // TODO - rename
    public interface IAgentCommandExecutionContext
    {
        IAgentCommand Command { get; }

        bool HasCommand { get; }
        bool Executed { get; }
        bool IsBusy { get; }

        //event EventHandler CommandComplete;

        void StartNewCommand(IGame game, IAgentCommand command);
        long Execute(IGame game, IAgent agent, long maxTimeSlice);

        long TimeRemaining { get; }
    }
}
