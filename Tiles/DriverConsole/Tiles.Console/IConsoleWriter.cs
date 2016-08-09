using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Console
{
    public interface IConsoleWriter
    {
        int BufferWidth { get; }
        int BufferHeight { get; }

        void SetCursorPosition(int x, int y);
        void SetForegroundColor(ConsoleColor color);
        void SetBackgroundColor(ConsoleColor color);
        void SetColors(ConsoleColor foreground, ConsoleColor background);

        void Write(string s);
        void Write(char c);
    }
}
