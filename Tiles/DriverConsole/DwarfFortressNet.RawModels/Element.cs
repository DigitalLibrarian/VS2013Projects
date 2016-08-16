using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    public class Element
    {
        public string Name { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
