using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles
{
    public interface IActionLog
    {
        // TODO - MaxLines should go away.  GetLines().Take(x) already does what we want.
        int MaxLines { get; }
        void AddLine(string line);
        IEnumerable<string> GetLines();
    }
}
