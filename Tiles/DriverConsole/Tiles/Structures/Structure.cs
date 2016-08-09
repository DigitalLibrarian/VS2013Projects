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
        public Vector2 Size { get; private set; }

        Dictionary<Vector2, IStructureCell> MyCells { get; set; }
        public IReadOnlyDictionary<Vector2, IStructureCell> Cells { get { return MyCells; } }

        public Structure(string name, Vector2 size)
        {
            Name = name;
            Size = size;

            MyCells = new Dictionary<Vector2, IStructureCell>();
        }

        public void Add(Vector2 relativePos, IStructureCell cell)
        {
            // TODO - fail if out of bounds (size)
            MyCells[relativePos] = cell;
        }
    }
}
