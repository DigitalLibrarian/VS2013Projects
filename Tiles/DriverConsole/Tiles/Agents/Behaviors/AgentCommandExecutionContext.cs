using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Behaviors
{
    public interface IActiveAgentCommandSet
    {
        long GetShortestTimeRemaining();
    }
    
    public interface IAgentContextStatusObserver
    {
        void StartCommand(IAgentCommandExecutionContext context);
        void EndCommand(IAgentCommandExecutionContext context);
    }

    public class AgentCommandContextTracker : IActiveAgentCommandSet, IAgentContextStatusObserver
    {
        public long GetShortestTimeRemaining()
        {
            return LiveContexts.Select(x => x.TimeRemaining).OrderBy(x => x).FirstOrDefault();
        }

        IList<IAgentCommandExecutionContext> LiveContexts { get; set; }
        public AgentCommandContextTracker()
        {
            LiveContexts = new List<IAgentCommandExecutionContext>();
        }

        void IAgentContextStatusObserver.StartCommand(IAgentCommandExecutionContext context)
        {
            LiveContexts.Add(context);
        }

        void IAgentContextStatusObserver.EndCommand(IAgentCommandExecutionContext context)
        {
            LiveContexts.Remove(context);
        }

    }

    public class AgentCommandExecutionContext : IAgentCommandExecutionContext
    {
        public IAgentCommand Command { get; private set; }
        IAgentCommandInterpreter Interpreter { get; set; }
        IAgentCommandPlanner Planner { get; set; }
        IAgentContextStatusObserver StatusObserver { get; set; }

        public long TimeRemaining { get; private set; }

        public bool Executed { get; private set; }
        public bool HasCommand { get { return Command != null; } }

        public AgentCommandExecutionContext(
            IAgentContextStatusObserver statusObserver,
            IAgentCommandPlanner planner,
            IAgentCommandInterpreter interpreter)
        {
            Planner = planner;
            Interpreter = interpreter;
            StatusObserver = statusObserver;
        }

        public void StartNewCommand(IGame game, IAgentCommand command)
        {
            Command = command;
            TimeRemaining = command.RequiredTime;
            Executed = false;
            StatusObserver.StartCommand(this);
        }

        public long Execute(IGame game, IAgent agent, long maxTimeSlice)
        {
            if (!HasCommand) return 0;

            long timeUsed = 0;

            if (TimeRemaining - maxTimeSlice > 0)
            {
                TimeRemaining -= maxTimeSlice;
                timeUsed = maxTimeSlice;
            }
            else
            {
                timeUsed = TimeRemaining;
                Interpreter.Execute(game, agent, Command);
                Executed = true;
                Command = null;
                StatusObserver.EndCommand(this);
                OnCommandComplete();
            }

            return timeUsed;
        }

        long ExecuteToCompletion(IGame game, IAgent agent)
        {
            Interpreter.Execute(game, agent, Command);
            Executed = true;
            Command = null;
            var timeUsed = TimeRemaining;
            TimeRemaining = 0;
            return timeUsed;
        }

        void PartialExecute(IGame game, IAgent agent, long timeUsed)
        {
            TimeRemaining -= timeUsed;
        }

        public void Update(IGame game, IAgent agent)
        {
            var maxTimeSlice = game.TicksPerUpdate;
            _Update(game, agent, maxTimeSlice);
        }

        public void _Update(IGame game, IAgent agent, long maxTimeSlice)
        {
            if (!HasCommand)
            {
                StartNewCommand(game, PlanNextCommand(game, agent));
            } 

            if (WillUseUpEntireFrame(maxTimeSlice))
            {
                TimeRemaining -= maxTimeSlice;
            }
            else
            {
                var timeLeftInFrame = maxTimeSlice - TimeRemaining;
                // takes less than a frame
                Invoke(game, agent);

                if (timeLeftInFrame >= TimeRemaining)
                {
                    _Update(game, agent, timeLeftInFrame);
                }
            }
            
            if (ShouldExecute())
            {
                Invoke(game, agent);
            }
        }

        IAgentCommand PlanNextCommand(IGame game, IAgent agent)
        {
            return Planner.PlanBehavior(game, agent);
        }


        bool WillUseUpEntireFrame(long frame)
        {
            return TimeRemaining - frame >= 0;
        }

        bool ShouldExecute()
        {
            return HasCommand && TimeRemaining <= 0;
        }

        void Invoke(IGame game, IAgent agent)
        {
            Interpreter.Execute(game, agent, Command);
            Executed = true;
            Command = null;
            TimeRemaining = 0;
            OnCommandComplete();
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
