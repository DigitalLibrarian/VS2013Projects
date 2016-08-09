using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Structures;
using Tiles.Math;

namespace Tiles
{
    public class Atlas : IAtlas
    {
        public Vector2 SiteSize { get; private set; }
        IDictionary<Vector2, ISite> Sites { get; set; }
        ISiteFactory SiteFactory { get; set; }

        public Atlas(ISiteFactory siteFactory, Vector2 siteSize)
        {
            Sites = new Dictionary<Vector2, ISite>();

            SiteFactory = siteFactory;
            SiteSize = siteSize;
        }

        public Vector2 SiteIndex(Vector2 pos)
        {
            return new Vector2(
                SignedDivideMap(pos.X, SiteSize.X),
                SignedDivideMap(pos.Y, SiteSize.Y));
        }

        Vector2 SiteTopLeft(Vector2 siteIndex)
        {
            return new Vector2(siteIndex.X * SiteSize.X, siteIndex.Y * SiteSize.Y);        
        }

        int SignedDivideMap(int n, int m)
        {
            if (n < 0)
            {
                n -= m - 1;
            }

            return n / m;
        }

        public Vector2 ToSitePos(Vector2 worldPos)
        {
            var index = SiteIndex(worldPos);
            var topLeft = SiteTopLeft(index);
            return worldPos - topLeft;
        }

        ISite GetSiteAtPos(Vector2 pos)
        {
            var siteIndex = SiteIndex(pos);
            var site = SiteLookup(siteIndex);
            if (site == null)
            {
                var origin = SiteTopLeft(siteIndex);
                var box = new Box(origin, origin + SiteSize);
                site = SiteFactory.Create(this, siteIndex, box);
                Sites.Add(siteIndex, site);
            }
            return site;
        }

        ISite SiteLookup(Vector2 index)
        {
            if (Sites.ContainsKey(index))
            {
                return Sites[index]; 
            }
            return null;
        }

        public IEnumerable<ITile> GetTiles(Box worldBox)
        {
            ISite site = null;
            Vector2 test, sitePos;
            for (int x = worldBox.Min.X; x <= worldBox.Max.X; x++)
            {
                for (int y = worldBox.Min.Y; y <= worldBox.Max.Y; y++)
                {
                    test = new Vector2(x, y);
                    if (site == null || !site.Box.Contains(test))
                    {
                        site = GetSiteAtPos(test);
                    }
                    sitePos = ToSitePos(test);
                    yield return site.GetTileAtSitePos(sitePos);
                }
            }
        }

        public IEnumerable<ITile> GetTiles()
        {
            return GetSites().SelectMany(site => site.GetTiles());
        }

        public IEnumerable<ISite> GetSites()
        {
            return Sites.Values;
        }

        public ITile GetTileAtPos(Vector2 pos)
        {
            var site = GetSiteAtPos(pos);
            var sitePos = ToSitePos(pos);

            return site.GetTileAtSitePos(sitePos);
        }
        
        public void InsertStructure(Vector2 pos, IStructure structure)
        {
            var size = structure.Size;
            for (int x = 0; x <= size.X; x++)
            {
                for (int y = 0; y <= size.Y; y++)
                {
                    var cellIndex = new Vector2(x, y);
                    var cell = structure.Cells[cellIndex];

                    var tile = GetTileAtPos(pos + cellIndex);
                    tile.StructureCell = cell;
                    tile.Terrain = Terrain.None;
                    tile.IsTerrainPassable = true;
                }
            }
        }
    }
}
