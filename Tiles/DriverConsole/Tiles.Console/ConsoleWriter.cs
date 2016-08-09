using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Inner = System.Console;

namespace Tiles.Console
{
    public class ConsoleWriter : IConsoleWriter
    {
        public int BufferWidth
        {
            get { return Inner.BufferWidth; }
        }

        public int BufferHeight
        {
            get { return Inner.BufferHeight; }
        }

        public void SetCursorPosition(int x, int y)
        {
            Inner.SetCursorPosition(x, y);
        }

        public void SetForegroundColor(ConsoleColor color)
        {
            Inner.ForegroundColor = color;
        }

        public void SetBackgroundColor(ConsoleColor color)
        {
            Inner.BackgroundColor = color;
        }

        public void SetColors(ConsoleColor foreground, ConsoleColor background)
        {
            SetForegroundColor(foreground);
            SetBackgroundColor(background);
        }

        public void Write(string s)
        {
            Inner.Write(s);
        }

        public void Write(char c)
        {
            Inner.Write(c);
        }
    }
}
