using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Parsing
{
    public interface IDfTagParser
    {
        IEnumerable<DfTag> Parse(string line);
    }
}
