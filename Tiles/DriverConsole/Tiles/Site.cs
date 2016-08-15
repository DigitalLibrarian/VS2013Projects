using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Structures;
using Tiles.Math;

namespace Tiles
{
    public class Site : ISite
    {
        public Box3 Box { get; private set; }
        public Vector3 Size { get { return Box.Size; } }
        ITile[][][] Tiles { get; set; }
        public Site(Box3 box)
        {
            Box = box;
            Allocate();
        }

        void Allocate()
        {
            Tiles = new ITile[Size.X][][];
            for (int x = 0; x < Size.X; x++)
            {
                Tiles[x] = new ITile[Size.Y][];
                for (int y = 0; y < Size.Y; y++)
                {
                    Tiles[x][y] = new ITile[Size.Z];
                    for (int z = 0; z < Size.Z; z++)
                    {
                        Tiles[x][y][z] = new Tile(x, y, z);
                    }
                }
            }
        }

        public bool InBounds(int x, int y, int z)
        {
            return x >= 0 && x < Size.X
                && y >= 0 && y < Size.Y
                && z >= 0 && z < Size.Z;
        }

        public bool InBounds(Vector3 v)
        {
            return InBounds(v.X, v.Y, v.Z);
        }

        public ITile GetTileAtIndex(int x, int y, int z)
        {
            if (!InBounds(x, y, z))
            {
                return null;
            }
            return Tiles[x][y][z];
        }

        public IEnumerable<ITile> GetTiles()
        {
            return Tiles.SelectMany(x => x).SelectMany(x => x);
        }

        public ITile GetTileAtSitePos(Vector3 index)
        {
            return GetTileAtIndex(index.X, index.Y, index.Z);
        }

        public void InsertStructure(Vector3 topLeftIndex, IStructure structure)
        {
            var size = structure.Size;
            for (int x = 0; x <= size.X; x++)
            {
                for (int y = 0; y <= size.Y; y++)
                {
                    for (int z = 0; z <= size.Z; z++)
                    {
                        var cellIndex = new Vector3(x, y, z);
                        if (structure.Cells.ContainsKey(cellIndex))
                        {
                            var cell = structure.Cells[cellIndex];

                            var siteIndex = topLeftIndex + cellIndex;
                            var tile = GetTileAtSitePos(siteIndex);
                            tile.StructureCell = cell;
                            tile.Terrain = Terrain.None;
                            tile.IsTerrainPassable = true;
                        }
                    }
                }
            }
        }
    }
}
