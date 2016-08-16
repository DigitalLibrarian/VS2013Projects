using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    public class Tag
    {
        public string Name { get { return Words.First(); } }
        public IList<string> Words { get; set; }
    }
}
