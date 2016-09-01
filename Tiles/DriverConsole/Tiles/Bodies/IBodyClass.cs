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
        /// <summary>
        /// Volume in cm3
        /// </summary>
        int Size { get; set; }
    }
}
