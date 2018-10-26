using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Behaviors;
using Tiles.Ecs;

namespace Tiles.EntityComponents
{
    public class CommandComponent : IComponent
    {
        public int Id { get { return ComponentTypes.Command; } }

        public IAgentBehavior Behavior { get; set; }

        public CommandComponent(IAgentBehavior behavior)
        {
            Behavior = behavior;
        }
    }
}
