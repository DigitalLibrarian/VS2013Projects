using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Ecs
{
    public interface IEntity
    {
        int Id { get; }
        bool HasComponent(int componentId);
        void AddComponent(IComponent comp);
        TComponent GetComponent<TComponent>(int componentId);
    }
}
