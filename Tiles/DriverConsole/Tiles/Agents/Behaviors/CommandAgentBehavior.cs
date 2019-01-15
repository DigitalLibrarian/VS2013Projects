using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;

namespace Tiles.Agents.Behaviors
{
    public class CommandAgentBehavior : IAgentBehavior
    {
        IAgentCommandPlanner Planner { get; set; }
        public IAgentCommandExecutionContext Context { get; set; }
        public CommandAgentBehavior(
            IAgentCommandPlanner planner, 
            IAgentCommandExecutionContext context)
        {
            Planner = planner;
            Context = context;
        }

        public void Update(IGame game, IAgent agent)
        {
            Update(game, agent, game.DesiredFrameLength);
        }

        void Update(IGame game, IAgent agent, long timeLeft)
        {
            while (timeLeft > 0)
            {
                if (timeLeft > 0 && !Context.HasCommand)
                {
                    Context.StartNewCommand(game, GetNextCommand(game, agent));
                }

                timeLeft -= Context.Execute(game, agent, timeLeft);

                if (!Context.HasCommand && agent.CommandQueue.Any())
                {
                    Context.StartNewCommand(game, GetNextCommand(game, agent));
                }
            }
        }

        private IAgentCommand GetNextCommand(IGame game, IAgent agent)
        {
            if (agent.CommandQueue.Any())
            {
                return agent.CommandQueue.Next();
            }
            else
            {
                foreach (var command in Planner.PlanBehavior(game, agent))
                {
                    agent.CommandQueue.Enqueue(command);
                }
                if (!agent.CommandQueue.Any())
                {
                    throw new InvalidOperationException("Planner failed to plan any commands");
                }
                return agent.CommandQueue.Next();
            }
        }
    }
}
