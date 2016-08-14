using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Items;

namespace Tiles.Agents
{
    public interface IAgentReaper
    {
        IEnumerable<IItem> Reap(IAgent agent);
        IEnumerable<IItem> Reap(IAgent agent, IBodyPart bodyPart);
    }
}
