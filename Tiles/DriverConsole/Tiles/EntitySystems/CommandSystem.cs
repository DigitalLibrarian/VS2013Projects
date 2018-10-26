using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Behaviors;
using Tiles.Ecs;
using Tiles.EntityComponents;

namespace Tiles.EntitySystems
{
    public class CommandSystem : AtlasBoxSystem
    {
        public CommandSystem()
            : base(ComponentTypes.Agent, 
                    ComponentTypes.Command)
        {

        }

        protected override void UpdateEntity(IEntityManager entityManager, IEntity entity, IGame game)
        {
            var agentComp = entity.GetComponent<IAgentComponent>(ComponentTypes.Agent);
            var commandComp = entity.GetComponent<ICommandComponent>(ComponentTypes.Command);

            var agent = agentComp.Agent;
            commandComp.Behavior.Update(game, agent);
        }
    }
}
