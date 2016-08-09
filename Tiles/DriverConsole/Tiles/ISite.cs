using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Structures;

namespace Tiles
{
    public interface ISite
    {
        Box Box { get; }
        ITile GetTileAtSitePos(Vector2 sitePos);
        bool InBounds(int x, int y);
        bool InBounds(Vector2 index);
        void InsertStructure(Vector2 topLeftIndex, IStructure structure);
        IEnumerable<ITile> GetTiles();
    }
}
