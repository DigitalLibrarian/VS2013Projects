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
            foreach (var tile0 in site.GetTiles())
            {
                tile0.TerrainSprite = new Sprite(Symbol.None, Color.White, Color.Black);
                tile0.IsTerrainPassable = true;
                tile0.Terrain = Terrain.None;
            }
            ITile tile;

            // Create 3 layers of impassable tiles
            int floorZBottom = 0, floorZTop = 3;
            for (int x = 0; x < site.Box.Size.X; x++)
            {
                for (int y = 0; y < site.Box.Size.Y; y++)
                {
                    for (int z = floorZBottom; z <= floorZTop; z++)
                    {
                        tile = site.GetTileAtIndex(x, y, z);
                        tile.IsTerrainPassable = false;
                        tile.TerrainSprite = new Sprite(Symbol.Terrain_Floor, Color.White, Color.White);
                    }
                }
            }

            // play snake on the top layer of "floor".  cut out a place for a river to form, with a drain at the start.
            var pos = new Vector3(box.Size.X / 2, box.Size.Y / 2, floorZTop);
            int maxRiverLength = 50;
            int riverLength = 0;
            int minLeg = 3, maxLeg = 10;

            var compass = new Vector3[]{
                CompassVectors.North,
                CompassVectors.East,
                CompassVectors.South,
                CompassVectors.West
            };


            tile = site.GetTileAtSitePos(pos);
            tile.IsTerrainPassable = true;
            tile.TerrainSprite = new Sprite(Symbol.None, Color.Black, Color.Black);

            Vector3 lastPlaced= Vector3.Zero;
            int numAttempts = 0, maxAttempts = 10;
            
            while (numAttempts < maxAttempts &&  riverLength < maxRiverLength)
            {
                var dir = Random.NextElement<Vector3>(compass);
                var legLength = Random.Next(minLeg, maxLeg);

                bool nerp = false;
                var test = pos + dir + dir;
                // scan ahead to look for problems
                for (int i = 0; i < legLength - 1; i++)
                {
                    tile = site.GetTileAtSitePos(test);
                    if (tile == null) continue;
                    if (tile.IsTerrainPassable) continue; // is this way already open?

                    foreach (var off in CompassVectors.GetAll())
                    {
                        tile = site.GetTileAtSitePos(test + off);
                        nerp = tile == null || tile.IsTerrainPassable;
                        if (nerp) break;
                    }
                    if (nerp) break;
                    test += dir;
                }
                if (nerp)
                {
                    numAttempts++;
                    continue;
                }

                for (int i = 0; i < legLength; i++)
                {
                    tile = site.GetTileAtSitePos(pos + dir);
                    if (tile == null) break;
                    if (tile.IsTerrainPassable) break; // is this way already open?

                    tile.IsTerrainPassable = true;
                    tile.TerrainSprite = new Sprite(Symbol.None, Color.Black, Color.Black);
                    riverLength++;
                    lastPlaced = pos + dir;
                    pos += dir;
                }
            }

            if (riverLength > 0)
            {
                for (int i = lastPlaced.Z; i >= 0; i--)
                {
                    tile = site.GetTileAtSitePos(lastPlaced + new Vector3(0, 0, -i));
                    tile.IsTerrainPassable = true;
                    tile.TerrainSprite = new Sprite(Symbol.None, Color.Black, Color.Black);
                }
            }

            const int space = 2;
            int waterZStart = floorZTop + 3, waterZThick = 5;
            var half = site.Size.X / 5;
            var mid = site.Size * 0.5d;
            for (int z = waterZStart; z < waterZStart + waterZThick; z++)
            {
                for (int x = mid.X - half; x < mid.X + half; x++)
                {
                    for (int y = mid.Y - half; y < mid.Y + half; y++)
                    {
                        if (x % space == 0 && y % space == 0)
                        {
                            tile = site.GetTileAtIndex(x, y, z);
                            tile.LiquidDepth = Random.Next(7, 7);
                            LiquidsSystem.CreateLiquidsNode(EntityManager, site, tile);
                        }
                    }
                }
            }


            return site;
        }
    }
}
