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
        Box3 Box { get; }

        void InsertStructure(Vector3 topLeftIndex, IStructure structure);

        ITile GetTileAtSitePos(Vector3 sitePos);

        IEnumerable<ITile> GetTiles();
    }
}
