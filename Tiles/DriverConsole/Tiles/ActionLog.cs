using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles
{
    // TODO - The "Capping" is a presentation logic and should be handled there.  The underlying list of log messages should 
    // be unbounded.
    public class ActionLog : IActionLog
    {
        List<string> LogLines { get; set; }
        public ActionLog()
        {
            LogLines = new List<string>();
        }

        public void AddLine(string line)
        {
            LogLines.Add(line);
        }

        public IEnumerable<string> GetLines()
        {
            return LogLines;
        }
    }
}
