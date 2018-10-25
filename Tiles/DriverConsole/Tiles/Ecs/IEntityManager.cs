using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Ecs
{
    public interface IEntityManager
    {
        IEntity CreateEntity();
        IEntity CreateEntity(int entityId);

        IEntity GetEntity(int entityId);
        IEntity DeleteEntity(int entityId);
        IEnumerable<IEntity> GetEntities(IEnumerable<int> componentIds);
    }
}
