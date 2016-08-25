using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Behaviors;
using Tiles.Math;

namespace Tiles.Agents
{
    public interface IAgentFactory
    {
        IAgent Create(IAtlas atlas, IAgentClass agentClass, Vector3 pos, IAgentCommandPlanner planner);
    }
}
