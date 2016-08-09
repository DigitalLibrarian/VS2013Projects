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
        public Box Box { get; private set; }
        public Vector2 Size { get { return Box.Size; } }
        ITile[][] Tiles { get; set; }
        public Site(Box box)
        {
            Box = box;
            Allocate();
        }

        void Allocate()
        {
            Tiles = new ITile[Size.X][];
            for (int x = 0; x < Size.X; x++)
            {
                Tiles[x] = new ITile[Size.Y];
                for (int y = 0; y < Size.Y; y++)
                {
                    Tiles[x][y] = new Tile(x, y);
                }
            }
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < Size.X
                && y >= 0 && y < Size.Y;
        }

        public bool InBounds(Vector2 v)
        {
            return InBounds(v.X, v.Y);
        }

        public ITile GetTileAtIndex(int x, int y)
        {
            if (!InBounds(x, y))
            {
                return null;
            }
            return Tiles[x][y];
        }

        public IEnumerable<ITile> GetTiles()
        {
            return Tiles.SelectMany(x => x);
        }

        public ITile GetTileAtSitePos(Vector2 index)
        {
            return GetTileAtIndex(index.X, index.Y);
        }

        public void InsertStructure(Vector2 topLeftIndex, IStructure structure)
        {
            var size = structure.Size;
            for (int x = 0; x <= size.X; x++)
            {
                for (int y = 0; y <= size.Y; y++)
                {
                    var cellIndex = new Vector2(x, y);
                    var cell = structure.Cells[cellIndex];

                    var tile = GetTileAtSitePos(topLeftIndex + cellIndex);
                    tile.StructureCell = cell;
                    tile.Terrain = Terrain.None;
                    tile.IsTerrainPassable = true;
                }
            }
        }
    }
}
