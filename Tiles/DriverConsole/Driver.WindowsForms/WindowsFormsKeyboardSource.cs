using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tiles.Control;

namespace Driver.WindowsForms
{
    public class WindowsFormsKeyboardSource : IKeyboadSource
    {
        private bool _keyAvail = false;
        public bool KeyAvailable
        {
            get { return _keyAvail; }
        }

        public event EventHandler<Tiles.Control.KeyPressEventArgs> KeyPressed;
        protected virtual void OnKeyPressed(Tiles.Control.KeyPressEventArgs e)
        {
            if (KeyPressed != null)
            {
                KeyPressed(this, e);
            }
        }

        public WindowsFormsKeyboardSource()
        {

        }

        Tiles.Control.KeyPressEventArgs MapEvent(KeyEventArgs e)
        {
            return new Tiles.Control.KeyPressEventArgs(
                (ConsoleKey)(int)e.KeyData, 
                (char)e.KeyCode, 
                e.Alt, e.Shift, e.Control);
        }

        public void InputKeyPressed(object sender, KeyEventArgs e)
        {
            _keyAvail = true;
            OnKeyPressed(MapEvent(e));
            _keyAvail = false;
        }
    }
}
