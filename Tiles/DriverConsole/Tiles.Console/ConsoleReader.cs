using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Inner = System.Console;

namespace Tiles.Console
{
    public class ConsoleReader : IConsoleReader
    {
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return Inner.ReadKey(intercept);
        }

        public string ReadLine()
        {
            return Inner.ReadLine();
        }

        public bool KeyAvailable
        {
            get { return Inner.KeyAvailable; }
        }
    }
}
