using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Ecs
{
    public class EntityManager : IEntityManager
    {
        Dictionary<int, IEntity> Entities { get; set; }

        public EntityManager()
        {
            Entities = new Dictionary<int, IEntity>();
        }

        
        public IEntity CreateEntity()
        {
            int id = Entities.Keys.OrderBy(x => x).LastOrDefault() + 1;
            return CreateEntity(id);
        }
        public IEntity CreateEntity(int entityId)
        {
            var entity = new Entity(entityId);
            Entities.Add(entityId, entity);
            return entity;
        }

        public IEntity GetEntity(int entityId)
        {
            if (!Entities.ContainsKey(entityId)) return null;

            return Entities[entityId];
        }

        public IEnumerable<IEntity> GetEntities(IEnumerable<int> componentIds)
        {
            return Entities.Values.Where(
                        entity => {
                            foreach(var reqId in componentIds)
                            {
                                if (!entity.HasComponent(reqId))
                                {
                                    return false;
                                }
                            }
                            return true;
                        });
        }
    }
}
