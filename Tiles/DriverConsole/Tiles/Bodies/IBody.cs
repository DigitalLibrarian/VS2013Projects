using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public interface IBody
    {
        IList<IBodyPart> Parts { get; }
        IBodyPart DamagePart(IBodyPart part, uint dmg);
    }
}
