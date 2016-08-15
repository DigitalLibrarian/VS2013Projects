using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Structures
{
    public class Structure : IStructure
    {
        public string Name { get; set; }
        public Vector3 Size { get; private set; }

        Dictionary<Vector3, IStructureCell> MyCells { get; set; }
        public IReadOnlyDictionary<Vector3, IStructureCell> Cells { get { return MyCells; } }

        public Structure(string name, Vector3 size)
        {
            Name = name;
            Size = size;

            MyCells = new Dictionary<Vector3, IStructureCell>();
        }

        public void Add(Vector3 relativePos, IStructureCell cell)
        {
            // TODO - fail if out of bounds (size)
            MyCells[relativePos] = cell;
        }
    }
}
