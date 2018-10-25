using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.EntityComponents;
using Tiles.EntitySystems;
using Tiles.Math;
using Tiles.Random;

namespace Tiles.ScreensImpl.SiteFactories
{
    public class LiquidsDemoSiteFactory : ISiteFactory
    {
        IEntityManager EntityManager { get; set; }
        IRandom Random { get; set; }
        public LiquidsDemoSiteFactory(IEntityManager entity, IRandom random)
        {
            EntityManager = entity;
            Random = random;
        }

        public ISite Create(IAtlas atlas, Math.Vector3 siteIndex, Math.Box3 box)
        {
            var site = new Site(box);
            foreach (var tile in site.GetTiles())
            {
                tile.TerrainSprite = new Sprite(Symbol.None, Color.White, Color.Black);
                tile.IsTerrainPassable = true;
                tile.Terrain = Terrain.None;
            }

            const int space = 5;
            int waterZStart = 3, waterZThick = 5;
            var half = site.Size.X/2;
            var mid = site.Size * 0.5d;
            for (int z = waterZStart; z < waterZStart + waterZThick; z++)
            {
                for (int x = mid.X - half; x < mid.X + half; x++)
                {
                    for (int y = mid.Y - half; y < mid.Y + half; y++)
                    {
                        if (x % space == 0 && y % space == 0)
                        {
                            var tile = site.GetTileAtIndex(x, y, z);
                            tile.LiquidDepth = Random.Next(1, 7);
                            LiquidsSystem.CreateLiquidsNode(EntityManager, site, tile);
                        }
                    }
                }
            }

            int floorZ = 0;
            for (int x = 0; x < site.Box.Size.X; x++)
            {
                for (int y = 0; y < site.Box.Size.Y; y++)
                {
                    var tile = site.GetTileAtIndex(x, y, floorZ);
                    tile.IsTerrainPassable = false;
                    if (Random.Next(5) == 1)
                    {
                        tile = site.GetTileAtIndex(x, y, floorZ+1);
                        tile.IsTerrainPassable = false;
                        tile.TerrainSprite = new Sprite(Symbol.Terrain_Floor, Color.White, Color.White);
                    }
                }
            }

            return site;
        }
    }
}
