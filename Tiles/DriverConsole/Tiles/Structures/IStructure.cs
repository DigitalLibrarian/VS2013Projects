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
        Vector3 Size { get; }

        IReadOnlyDictionary<Vector3, IStructureCell> Cells { get; }
    }
}
