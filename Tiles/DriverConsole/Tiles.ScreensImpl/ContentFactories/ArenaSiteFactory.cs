using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Random;

namespace Tiles.ScreensImpl.ContentFactories
{
    class ArenaSiteFactory : ISiteFactory
    {
        IRandom Random { get; set; }

        public ArenaSiteFactory(IRandom random)
        {
            // TODO: Complete member initialization
            this.Random = random;
        }
        public ISite Create(IAtlas atlas, Math.Vector3 siteIndex, Box3 box)
        {
            var s = new Site(box);

            foreach (var tile in s.GetTiles())
            {
                tile.TerrainSprite = new Sprite(Symbol.None, Color.White, Color.Black);
                tile.IsTerrainPassable = true;
                tile.Terrain = Terrain.None;
            }
            var structureFactory = new StructureFactory();
            var structure = structureFactory.CreateRectangularBuilding(box.Size - new Vector3(1, 1, 1));
            s.InsertStructure(Vector3.Zero, structure);

            return s;
        }
    }
}
