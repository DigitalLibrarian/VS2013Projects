using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Ecs;

namespace Tiles.EntityComponents
{
    public class AgentComponent : IComponent
    {
        public int Id { get { return ComponentTypes.Agent; } }
        public IAgent Agent { get; set; }

        public AgentComponent(IAgent agent)
        {
            Agent = agent;
        }
    }
}
