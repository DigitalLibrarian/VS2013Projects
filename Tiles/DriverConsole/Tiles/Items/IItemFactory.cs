using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Math;

namespace Tiles.Items
{

    public interface IItemFactory
    {
        IItem Create(IItemClass itemClass);
        IItem CreateShedLimb(IAgent agent, IBodyPart part);
        IItem CreateCorpse(IAgent agent);
    }

}
