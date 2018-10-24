using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Random;

namespace Tiles.ScreensImpl.SiteFactories
{
    public class LiquidsDemoSiteFactory : ISiteFactory
    {
        IRandom Random { get; set; }
        public LiquidsDemoSiteFactory(IRandom random)
        {
            Random = random;
        }

        public ISite Create(IAtlas atlas, Math.Vector3 siteIndex, Math.Box3 box)
        {
            var s = new Site(box);
            foreach (var tile in s.GetTiles())
            {
                tile.TerrainSprite = new Sprite(Symbol.None, Color.White, Color.Black);
                tile.IsTerrainPassable = true;
                tile.Terrain = Terrain.None;
            }

            var z = 1;
            var half = s.Size.X/2;
            var mid = s.Size * 0.5d;
            for (int x = mid.X - half; x < mid.X + half; x++)
            {
                for (int y = mid.Y - half; y < mid.Y + half; y++)
                {
                    const int space = 3;
                    if (x % space == 0 && y % space == 0)
                    {
                        var tile = s.GetTileAtIndex(x, y, z);
                        if (tile == null)
                        {
                            var brak = 9;
                        }
                        tile.LiquidDepth = Random.Next(1, 7);
                    }
                }
            }

            return s;
        }
    }
}
