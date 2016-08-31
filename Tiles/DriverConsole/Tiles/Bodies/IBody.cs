using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies.Health;
using Tiles.Bodies.Health.Injuries;

namespace Tiles.Bodies
{
    public interface IBody
    {
        bool IsGrasping { get; }
        bool IsBeingGrasped { get; }
        bool IsWrestling { get; }
        int Size { get; }
        IList<IBodyPart> Parts { get; }

        void Amputate(IBodyPart part);

        void AddInjuries(IEnumerable<IInjury> injuries);

        IHealthState Health { get; }
    }
}
