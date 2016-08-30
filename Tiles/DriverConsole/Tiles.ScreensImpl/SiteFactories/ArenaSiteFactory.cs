using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.Math;
using Tiles.Random;
using Tiles.ScreensImpl.ContentFactories;

namespace Tiles.ScreensImpl.SiteFactories
{
    class ArenaSiteFactory : ISiteFactory
    {
        IEntityManager EntityManager { get; set; }
        IRandom Random { get; set; }
        DfTagsFascade Df { get; set; }

        public ArenaSiteFactory(IDfObjectStore store, IEntityManager entityManager, IRandom random)
        {
            EntityManager = entityManager;
            this.Random = random;

            Df = new DfTagsFascade(store, entityManager, random);
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

            var testSpot = new Vector3(1, 1, 1);

            if (!box.Contains(testSpot))
            {
                return s;
            }
            var buildingBox = new Box3(Vector3.Zero, new Vector3(box.Size.X - 1, box.Size.Y - 1, 1));
            var structureFactory = new StructureFactory();
            var structure = structureFactory.CreateRectangularBuilding(
                buildingBox.Size, buildingColor: Color.DarkRed, notBuildingColor: Color.Black);

            s.InsertStructure(buildingBox.Min, structure);

            var pos = Random.FindRandomInBox(buildingBox, test => s.GetTileAtSitePos(test).HasRoomForAgent).Value;

            var troll = Df.CreateCreatureAgent(atlas, "TROLL", "MALE", s.Box.Min + pos);
            EcsBridge.Bridge(troll, EntityManager);

            s.GetTileAtSitePos(pos).SetAgent(troll);


            return s;
        }
    }
}
