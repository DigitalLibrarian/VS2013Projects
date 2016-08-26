using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Behaviors;
using Tiles.Agents.CommandInterpreters;
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

        public IAgent Create(IAtlas atlas, IAgentClass agentClass, Vector3 pos, IAgentCommandPlanner planner)
        {
            var body = BodyFactory.Create(agentClass.BodyClass);
            var agent = new Agent(
                atlas,
                agentClass,
                pos,
                body,
                new Inventory(),
                new Outfit(body, new OutfitLayerFactory()),
                new AgentCommandQueue()
                );
            agent.AgentBehavior = CreateBehavior(planner);
            return agent;
        }

        static IAgentCommandInterpreter CommandInterpreter = new DefaultAgentCommandInterpreter();

        IAgentBehavior CreateBehavior(IAgentCommandPlanner planner)
        {
            return new CommandAgentBehavior(planner, new AgentCommandExecutionContext(CommandInterpreter));
        }
    }
}
