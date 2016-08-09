using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Control
{    
    public interface IKeyboadSource
    {
        bool KeyAvailable { get; }
        event EventHandler<KeyPressEventArgs> KeyPressed;
    }
}
