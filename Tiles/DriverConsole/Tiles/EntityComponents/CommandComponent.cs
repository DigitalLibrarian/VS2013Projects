using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Behaviors;
using Tiles.Ecs;

namespace Tiles.EntityComponents
{
    public interface ICommandComponent : IComponent
    {
        IAgentCommandQueue CommandQueue { get; }
        IAgentCommandPlanner Planner { get; }
        IAgentCommandExecutionContext Context { get; }
    }

    public class CommandComponent : ICommandComponent
    {
        public int Id { get { return ComponentTypes.Command; } }

        public IAgentCommandQueue CommandQueue { get; set; }
        public IAgentCommandPlanner Planner { get; set; }
        public IAgentCommandExecutionContext Context { get; set; }
    }
}
