using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Behaviors
{
    public class AgentCommandExecutionContext : IAgentCommandExecutionContext
    {
        public IAgentCommand Command { get; set; }
        IAgentCommandInterpreter Interpreter { get; set; }

        public long TimeRemaining { get; set; }

        public bool Executed { get; private set; }
        public bool HasCommand { get { return Command != null; } }
        public bool IsBusy
        {
            get
            {
                return HasCommand && !Executed;
            }
        }


        public AgentCommandExecutionContext(IAgentCommandInterpreter interpreter)
        {
            Interpreter = interpreter;
        }

        public void StartNewCommand(IGame game, IAgentCommand command)
        {
            Command = command;
            TimeRemaining = command.RequiredTime;
            Executed = false;
        }

        public long Execute(IGame game, IAgent agent, long maxTimeSlice)
        {
            long timeUsed = 0;
            if (!HasCommand) return 0;
            if (TimeRemaining - maxTimeSlice > 0)
            {
                TimeRemaining -= maxTimeSlice;
                timeUsed = maxTimeSlice;
            }
            else
            {
                timeUsed = System.Math.Min(maxTimeSlice, TimeRemaining);
                TimeRemaining = 0;
                Interpreter.Execute(game, agent, Command);
                Executed = true;
                Command = null;
                OnCommandComplete();
            }

            return timeUsed;
        }


        public event EventHandler CommandComplete;
        protected void OnCommandComplete()
        {
            if (CommandComplete != null)
            {
                CommandComplete(this, EventArgs.Empty);
            }
        }
    }
}
