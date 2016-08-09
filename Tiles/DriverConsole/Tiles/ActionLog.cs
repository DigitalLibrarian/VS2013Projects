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
        CappedRollingList<string> LogLines { get; set;}
        public int MaxLines { get { return LogLines.Max; } }
        public ActionLog(int maxLines)
        {
            LogLines = new CappedRollingList<string>(maxLines);
        }

        public void AddLine(string line)
        {
            LogLines.Add(line);
        }

        public IEnumerable<string> GetLines()
        {
            return LogLines.Get();
        }
    }

    public class CappedRollingList<T>
    {
        public int Max { get; private set; }
        List<T> List { get; set; }

        public CappedRollingList(int max)
        {
            Max = max;
            List = new List<T>(Max);
        }

        public void Add(T thing)
        {
            if (List.Count() == Max)
            {
                List.RemoveAt(0);
            }

            List.Add(thing);
        }

        public IEnumerable<T> Get()
        {
            return List;
        }
    }
}
