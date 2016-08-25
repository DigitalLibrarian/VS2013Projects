using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public interface IBodyClassFactory
    {
        IBodyClass CreateHumanoid();
        IBodyClass CreateFeralHumanoid();
    }
}
