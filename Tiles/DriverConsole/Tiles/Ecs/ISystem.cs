using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Ecs
{
    public interface ISystem
    {
        // This might become a list of types
        IEnumerable<int> ComponentIds { get; }

        void Update(IEntityManager entityManager, IGame game);
    }
}
