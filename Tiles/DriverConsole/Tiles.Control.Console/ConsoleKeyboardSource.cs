using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Console;
using Tiles.Control;
using Tiles.Math;
namespace Tiles.Control.Console
{
    public class ConsoleKeyboardSource : IKeyboadSource
    {
        IConsoleReader Reader { get; set; }
        public bool KeyAvailable { get { return Reader.KeyAvailable; } }
        public ConsoleKeyboardSource(IConsoleReader reader)
        {
            Reader = reader;
        }

        public void Pump() 
        {
            var input = Reader.ReadKey(intercept: true);
            OnKeyPressed(
                new KeyPressEventArgs(input.Key, input.KeyChar,
                    input.Modifiers.HasFlag(ConsoleModifiers.Alt),
                    input.Modifiers.HasFlag(ConsoleModifiers.Shift),
                    input.Modifiers.HasFlag(ConsoleModifiers.Control)));
        }

        public event EventHandler<KeyPressEventArgs> KeyPressed;
        protected virtual void OnKeyPressed(KeyPressEventArgs e)
        {
            if (KeyPressed != null)
            {
                KeyPressed(this, e);
            }
        }
    }

}
