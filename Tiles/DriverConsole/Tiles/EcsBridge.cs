using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Ecs;
using Tiles.EntityComponents;

namespace Tiles
{
    // temporary
    public static class EcsBridge
    {
        public static int Bridge(IAgent agent, IEntityManager entityManager)
        {
            var entity = entityManager.CreateEntity();

            entity.AddComponent(
                new AgentComponent(agent));

            entity.AddComponent(
                new CommandComponent(agent.AgentBehavior));

            entity.AddComponent(
                new AtlasPositionComponent(
                    new AtlasPosition(() => agent.Pos)));
            if(agent.EntityId != 0)
            {
                throw new InvalidOperationException();
            }
            agent.EntityId = entity.Id;
            return entity.Id;
        }
    }
}
