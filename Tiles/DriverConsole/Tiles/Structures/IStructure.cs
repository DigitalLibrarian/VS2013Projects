using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Structures
{
    public interface IStructure
    {
        string Name { get; set; }
        Vector2 Size { get; }
        //Box IndexBox { get; }

        IReadOnlyDictionary<Vector2, IStructureCell> Cells { get; }
    }
}
