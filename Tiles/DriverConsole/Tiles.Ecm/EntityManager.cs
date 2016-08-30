using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Ecm
{
    public class EntityManager : IEntityManager
    {
        Dictionary<int, List<IComponent>> EntityComponents { get; set; }

        public EntityManager()
        {
            EntityComponents = new Dictionary<int, List<IComponent>>();
        }

        public void AddEntity(int entityId)
        {
            EntityComponents.Add(0, new List<IComponent>());
        }

        public void AddComponent(int entityId, IComponent component)
        {
            EntityComponents[entityId].Add(component);
        }

        public TComponent GetComponent<TComponent>(int entityId) where TComponent : class
        {
            var comp = EntityComponents[entityId].FirstOrDefault(c => c is TComponent);
            return comp as TComponent;
        }
    }
}
