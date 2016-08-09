using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Structures;

namespace Tiles
{
    public interface IAtlas
    {
        Vector2 SiteSize { get; }

        void InsertStructure(Vector2 topLeftIndex, IStructure structure);

        ITile GetTileAtPos(Vector2 pos);

        IEnumerable<ITile> GetTiles();
        IEnumerable<ISite> GetSites();

        IEnumerable<ITile> GetTiles(Box worldBox);
    }
}
