using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.ScreensImpl.ContentFactories
{
    public interface IBodyClassFactory
    {
        IBodyClass CreateHumanoid();
        IBodyClass CreateFeralHumanoid();
    }
}
