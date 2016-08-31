using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public interface IBodyClass
    {
        IEnumerable<IBodyPartClass> Parts { get; set; }
        int Size { get; set; }
    }
}
