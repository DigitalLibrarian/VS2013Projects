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
            int floorZBottom = 0, floorZTop = 6;
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
            var pos = new Vector3(box.Size.X / 2, box.Size.Y / 2, floorZTop-3);
            var riverStartPos = pos;
            int maxRiverLength = 15;
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

                    var sideDir = new Vector3(
                        dir.X == 0 ? 1 : 0,
                        dir.Y == 0 ? 1 : 0,
                        0);
                    var up = new Vector3(0, 0, 1);
                    tile = site.GetTileAtSitePos(pos + dir + sideDir + up);
                    if (tile != null && tile.IsTerrainPassable)
                    {
                        tile.IsTerrainPassable = false;
                        tile.TerrainSprite = new Sprite(Symbol.Terrain_Floor, Color.White, Color.White);
                    }

                    tile = site.GetTileAtSitePos(pos + dir - sideDir + up);
                    if (tile != null && tile.IsTerrainPassable)
                    {
                        tile.IsTerrainPassable = false;
                        tile.TerrainSprite = new Sprite(Symbol.Terrain_Floor, Color.White, Color.White);
                    }

                    tile = site.GetTileAtSitePos(pos - dir + sideDir + up);
                    if (tile != null && tile.IsTerrainPassable)
                    {
                        tile.IsTerrainPassable = false;
                        tile.TerrainSprite = new Sprite(Symbol.Terrain_Floor, Color.White, Color.White);
                    }

                    tile = site.GetTileAtSitePos(pos - dir - sideDir + up);
                    if (tile != null && tile.IsTerrainPassable)
                    {
                        tile.IsTerrainPassable = false;
                        tile.TerrainSprite = new Sprite(Symbol.Terrain_Floor, Color.White, Color.White);
                    }

                    riverLength++;
                    lastPlaced = pos + dir;
                    pos += dir;
                }
            }

            if (riverLength > 0)
            {
                // Create spout going up, at the end of the river
                for (int i = lastPlaced.Z; i >= 0; i--)
                {
                    tile = site.GetTileAtSitePos(lastPlaced + new Vector3(0, 0, i));
                    tile.IsTerrainPassable = true;
                    tile.TerrainSprite = new Sprite(Symbol.None, Color.Black, Color.Black);

                    for (int zOff = 1; zOff < 6; zOff++)
                        for (int xOff = -1; xOff < 2; xOff++)
                            for (int yOff = -1; yOff < 2; yOff++)
                            {
                                tile = site.GetTileAtSitePos(lastPlaced + new Vector3(xOff, yOff, zOff));
                                tile.IsTerrainPassable = true;
                                tile.TerrainSprite = new Sprite(Symbol.None, Color.Black, Color.Black);
                            }
                }
            }

            // create funnel opening
            for (int zOff = 0; zOff < 6; zOff++)
            {
                for (int xOff = -1 + -zOff; xOff < 2+zOff; xOff++)
                    for (int yOff = -1 + -zOff; yOff < 2+zOff; yOff++)
                    {
                        tile = site.GetTileAtSitePos(riverStartPos + new Vector3(xOff, yOff, zOff));
                        tile.IsTerrainPassable = true;
                        tile.TerrainSprite = new Sprite(Symbol.None, Color.Black, Color.Black);
                    }
            }

            const int space = 2;
            int waterZStart = floorZTop + 3, waterZThick = 5;
            var half = site.Size.X / 16;
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
