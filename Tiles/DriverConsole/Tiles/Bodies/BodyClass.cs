using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public class BodyClass : IBodyClass
    {
        public BodyClass(IEnumerable<IBodyPartClass> parts, int size)
        {
            Parts = parts;
            Size = size;
        }
        public IEnumerable<IBodyPartClass> Parts { get; set; }
        public int Size { get; set; }
    }
}
