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
        public CommandAgentBehavior(IAgentCommandPlanner planner, IAgentCommandExecutionContext context)
        {
            Planner = planner;
            Context = context;
        }

        public void Update(IGame game, IAgent agent)
        {
            Update(game, agent, game.TicksPerUpdate);
        }

        long Update(IGame game, IAgent agent, long maxTimeSlice)
        {
            if (!Context.HasCommand)
            {
                var command = Planner.PlanBehavior(game, agent);
                Context.StartNewCommand(game, command);
            }

            return Context.Execute(game, agent, maxTimeSlice);
        }
    }
}
