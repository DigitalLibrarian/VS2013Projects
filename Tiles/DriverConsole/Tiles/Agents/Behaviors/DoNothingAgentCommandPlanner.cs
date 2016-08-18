using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Behaviors
{
    public class DoNothingAgentCommandPlanner : IAgentCommandPlanner
    {
        IAgentCommandFactory CommandFactory { get; set; }
        public DoNothingAgentCommandPlanner(IAgentCommandFactory fact)
        {
            CommandFactory = fact;
        }

        public IEnumerable<IAgentCommand> PlanBehavior(IGame game, IAgent agent)
        {
            return CommandFactory.Nothing(agent);
        }
    }
}
