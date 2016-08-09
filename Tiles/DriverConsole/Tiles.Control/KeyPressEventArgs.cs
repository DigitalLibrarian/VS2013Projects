using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Control
{
    public class KeyPressEventArgs : EventArgs
    {
        public ConsoleKey Key { get; private set; }
        public char KeyChar { get; private set; }

        public bool Alt { get; private set; }
        public bool Shift { get; private set; }
        public bool Control { get; private set; }

        public KeyPressEventArgs(ConsoleKey key, char keyChar, bool alt, bool shift, bool control)
            : base()
        {
            Key = key;
            KeyChar = keyChar;
            Alt = alt;
            Shift = shift;
            Control = control;
        }
    }
}
