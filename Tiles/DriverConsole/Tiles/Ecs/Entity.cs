using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Ecs
{
    public class Entity : IEntity
    {
        public int Id { get; private set; }
        Dictionary<int, IComponent> Components { get; set; }

        public Entity(int id)
        {
            Id = id;
            Components = new Dictionary<int, IComponent>();
        }

        public bool HasComponent(int componentId)
        {
            return Components.ContainsKey(componentId);
        }

        public void AddComponent(IComponent comp)
        {
            Components.Add(comp.Id, comp);
        }
        
        public TComponent GetComponent<TComponent>(int componentId)
        {
            var comp = Components.Values.FirstOrDefault(c => c.Id == componentId);
            return (TComponent)comp;
        }
    }
}
