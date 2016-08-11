using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public interface IBody
    {
        bool IsGrasping { get; }
        bool IsWrestling { get; }
        IList<IBodyPart> Parts { get; }
        IBodyPart DamagePart(IBodyPart part, uint dmg);
    }
}
