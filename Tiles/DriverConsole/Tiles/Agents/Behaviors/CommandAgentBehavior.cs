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
        IAgentCommandInterpreter Interpreter { get; set; }
        IAgentCommandExecutionContext Context { get; set; }
        public CommandAgentBehavior(IAgentCommandPlanner planner, IAgentCommandInterpreter interpreter)
        {
            Planner = planner;
            Interpreter = interpreter;
        }

        public void Update(IGame game, IAgent agent)
        {
            Interpreter.Execute(game, agent, Planner.PlanBehavior(game, agent));
        }
    }
}
