using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

using Inner = System.Console;

namespace Tiles.Console
{
    public interface IConsoleReader
    {
        bool KeyAvailable { get; }
        ConsoleKeyInfo ReadKey(bool intercept);
        string ReadLine();
    }
}
