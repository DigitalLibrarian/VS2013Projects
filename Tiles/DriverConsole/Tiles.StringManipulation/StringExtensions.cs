using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.StringManipulation
{
    public static class StringExtensions
    {
        public static IEnumerable<string> WrapText(this IEnumerable<string> lines, int max)
        {
            foreach (var line in lines)
            {
                foreach (var frag in WrapLine(line, max))
                {
                    yield return frag;
                }
            }
        }

        public static IEnumerable<string> WrapLine(this string line, int max)
        {
            if (line.Count() > max)
            {
                yield return line.Substring(0, max);

                foreach (var frag in WrapLine(line.Substring(max, line.Count() - max), max))
                {
                    yield return frag;
                }
            }
            else
            {
                yield return line;
            }
        }
    }
}
