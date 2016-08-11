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
            Update(game, agent, game.DesiredFrameLength);
        }

        void Update(IGame game, IAgent agent, long maxTimeSlice)
        {
            var timeLeft = maxTimeSlice;

            while (timeLeft > 0)
            {

                if (!Context.HasCommand)
                {
                    var command = Planner.PlanBehavior(game, agent);
                    Context.StartNewCommand(game, command);
                }

                var timeUsed = Context.Execute(game, agent, maxTimeSlice);
                if (timeUsed == timeLeft)
                {
                    break;
                }
                timeLeft -= timeUsed;
            }
        }
    }
}
