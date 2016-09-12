using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles
{
    public interface IActionLog
    {
        void AddLine(string line);
        IEnumerable<string> GetLines();
    }
}
