using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles
{
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
