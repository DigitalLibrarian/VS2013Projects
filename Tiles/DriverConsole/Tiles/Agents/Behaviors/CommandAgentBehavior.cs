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
        public CommandAgentBehavior(IAgentCommandPlanner planner, IAgentCommandInterpreter interpreter)
        {
            Planner = planner;
            Interpreter = interpreter;
        }

        public void Update(IGame game, IAgent agent)
        {
            var command = Planner.PlanBehavior(game, agent);
            Interpreter.Execute(game, agent, command);
        }
    }

    public class TimeContinuum
    {
        public long Tick { get; private set; }

        void Update()
        {
            Tick++;
        }
    }
}
