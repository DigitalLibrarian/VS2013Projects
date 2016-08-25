using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Behaviors;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Items.Outfits;
using Tiles.Math;

namespace Tiles.Agents
{
    public class AgentFactory : IAgentFactory
    {
        IBodyFactory BodyFactory { get; set; }
        public AgentFactory(IBodyFactory bodyFactory)
        {
            BodyFactory = bodyFactory;
        }


        public IAgent Create(IAtlas atlas, IAgentClass agentClass, Vector3 pos)
        {
            var body = BodyFactory.Create(agentClass.BodyClass);
            return new Agent(
                atlas,
                agentClass,
                pos,
                body,
                new Inventory(),
                new Outfit(body, new OutfitLayerFactory()),
                new AgentCommandQueue()
                );
        }
    }
}
