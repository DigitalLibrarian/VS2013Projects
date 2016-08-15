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
        public Vector3 SiteSize { get; private set; }
        IDictionary<Vector3, ISite> Sites { get; set; }
        ISiteFactory SiteFactory { get; set; }

        public Atlas(ISiteFactory siteFactory, Vector3 siteSize)
        {
            Sites = new Dictionary<Vector3, ISite>();

            SiteFactory = siteFactory;
            SiteSize = siteSize;
        }

        public Vector3 SiteIndex(Vector3 pos)
        {
            return new Vector3(
                SignedDivideMap(pos.X, SiteSize.X),
                SignedDivideMap(pos.Y, SiteSize.Y),
                SignedDivideMap(pos.Z, SiteSize.Z));
        }

        Vector3 SiteTopLeft(Vector3 siteIndex)
        {
            return new Vector3(
                siteIndex.X * SiteSize.X,
                siteIndex.Y * SiteSize.Y,
                siteIndex.Z * SiteSize.Z);
        }

        int SignedDivideMap(int n, int m)
        {
            if (n < 0)
            {
                n -= m - 1;
            }

            return n / m;
        }

        public Vector3 ToSitePos(Vector3 worldPos)
        {
            var index = SiteIndex(worldPos);
            var topLeft = SiteTopLeft(index);
            return worldPos - topLeft;
        }

        ISite GetSiteAtPos(Vector3 pos)
        {
            var siteIndex = SiteIndex(pos);
            var site = SiteLookup(siteIndex);
            if (site == null)
            {
                var origin = SiteTopLeft(siteIndex);
                var box = new Box3(origin, origin + SiteSize);
                site = SiteFactory.Create(this, siteIndex, box);
                Sites.Add(siteIndex, site);
            }
            return site;
        }

        ISite SiteLookup(Vector3 index)
        {
            if (Sites.ContainsKey(index))
            {
                return Sites[index]; 
            }
            return null;
        }

        public IEnumerable<ITile> GetTiles(Box3 worldBox)
        {
            ISite site = null;
            Vector3 test, sitePos;
            for (int x = worldBox.Min.X; x <= worldBox.Max.X; x++)
            {
                for (int y = worldBox.Min.Y; y <= worldBox.Max.Y; y++)
                {
                    for (int z = worldBox.Min.Z; z <= worldBox.Max.Z; z++)
                    {
                        test = new Vector3(x, y, z);
                        if (site == null || !site.Box.Contains(test))
                        {
                            site = GetSiteAtPos(test);
                        }
                        sitePos = ToSitePos(test);
                        yield return site.GetTileAtSitePos(sitePos);

                    }
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

        public ITile GetTileAtPos(Vector3 pos)
        {
            var site = GetSiteAtPos(pos);
            var sitePos = ToSitePos(pos);

            return site.GetTileAtSitePos(sitePos);
        }
        
        public void InsertStructure(Vector3 pos, IStructure structure)
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

                            var worldPos = pos + cellIndex;
                            var tile = GetTileAtPos(worldPos);
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
