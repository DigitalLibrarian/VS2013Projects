using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Ecm
{
    public interface IComponent
    {
        int Id { get; }

        void Update(IEntityManager entityManager, int ticks);
    }

    public interface ISystem
    {
        IEnumerable<int> ComponentIds { get; }
    }

    public interface IEntityManager
    {
        void AddEntity(int entityId);
        void AddComponent(int entityId, IComponent component);

        TComponent GetComponent<TComponent>(int entityId) where TComponent : class;
    }
}
