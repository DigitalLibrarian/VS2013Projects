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
        Vector3 SiteSize { get; }

        void InsertStructure(Vector3 topLeftIndex, IStructure structure);

        ITile GetTileAtPos(Vector3 pos, bool autoViv=true);
        ISite GetSiteAtPos(Vector3 pos, bool autoViv=true);

        IEnumerable<ITile> GetTiles();
        IEnumerable<ISite> GetSites();

        IEnumerable<ITile> GetTiles(Box3 worldBox);
    }
}
