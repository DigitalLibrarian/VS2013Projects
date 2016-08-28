using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TilesControls = Tiles.Control;

namespace Driver.Tiles.WindowsForms
{
    public class WindowsFormsKeyboardSource : TilesControls.IKeyboadSource
    {
        private bool _keyAvail = false;
        public bool KeyAvailable
        {
            get { return _keyAvail; }
        }

        public event EventHandler<TilesControls.KeyPressEventArgs> KeyPressed;
        protected virtual void OnKeyPressed(TilesControls.KeyPressEventArgs e)
        {
            if (KeyPressed != null)
            {
                KeyPressed(this, e);
            }
        }

        public WindowsFormsKeyboardSource()
        {

        }

        TilesControls.KeyPressEventArgs MapEvent(KeyEventArgs e)
        {
            return new TilesControls.KeyPressEventArgs(
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
