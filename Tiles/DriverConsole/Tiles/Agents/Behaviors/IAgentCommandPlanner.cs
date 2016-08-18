using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles;
using Tiles.Math;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Random;
using Tiles.Bodies;
using Tiles.Items;

namespace Tiles.Agents.Behaviors
{
    public interface IAgentCommandPlanner
    {
        IEnumerable<IAgentCommand> PlanBehavior(IGame game, IAgent agent);
    }
}
