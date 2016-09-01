using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using Tiles.Control;
using Tiles.Items;
using Tiles.Render;
using Tiles.Math;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Agents.Behaviors;
using Tiles.Random;
using Tiles.Structures;
using DfNet.Raws;
using Tiles.ScreensImpl.SiteFactories;
using Tiles.Ecs;



namespace Tiles.ScreensImpl.ContentFactories
{
    public class GameFactory
    {
        public IGame SetupGenericZombieWorld(int seed = 42)
        {
            var entityManager = new EntityManager();

            Vector3 siteSize = new Vector3(64, 64, 64);
            var random = new RandomWrapper(new System.Random(seed));
            var siteFactory = new ZombieSiteFactory(entityManager, random);
            return Setup(entityManager, siteFactory, siteSize, random);
        }

        public IGame SetupArenaWorld(string dfRawDir, int seed = 42)
        {
            var entityManager = new EntityManager();
            var siteSize = new Vector3(64, 64, 64);
            var random = new RandomWrapper(new System.Random(seed));

            var dfStore = DfObjectStore.CreateFromDirectory(dfRawDir);
            var df = new DfTagsFascade(dfStore, entityManager, random);
            var sword = df.CreateWeapon("ITEM_WEAPON_SWORD_SHORT", "ADAMANTINE");

            var siteFactory = new ArenaSiteFactory(dfStore, entityManager, random);
            return Setup(entityManager, siteFactory, siteSize, random, sword);
        }

        public IGame SetupDfTestWorld(string dfRawDir, int seed = 42)
        {
            var entityManager = new EntityManager();
            var dfStore = DfObjectStore.CreateFromDirectory(dfRawDir);

            var siteSize = new Vector3(64, 64, 64);
            var random = new RandomWrapper(new System.Random(seed));
            var siteFactory = new DfTestSiteFactory(dfStore, entityManager, random);
            return Setup(entityManager, siteFactory, siteSize, random);

        }

        private Game Setup(IEntityManager entityManager,
            ISiteFactory siteFactory, Vector3 siteSize, IRandom random, 
            params IItem[] invItems)
        {
            var atlas = new Atlas(siteFactory, siteSize);

            var actionLog = new ActionLog(maxLines: 10);
            
            var spawnBox = new Box3(
                Vector3.Zero,
                new Vector3(siteSize.X, siteSize.Y, 1)
                );
            
            var spawnPos = FindSpawnLocation(atlas, random, spawnBox);
            var player = CreatePlayer(random, entityManager, atlas, spawnPos);

            foreach (var item in invItems)
            {
                player.Inventory.AddItem(item);
            }

            var camera = new Camera(spawnPos);

            return new Game(entityManager, atlas, player, camera, actionLog, random);
        }

        private IPlayer CreatePlayer(IRandom random, IEntityManager entityManager, IAtlas atlas, Vector3 spawnPos)
        {
            return new HardCodedAgentFactory(entityManager, random).CreatePlayer(atlas, spawnPos);
        }

        Vector3 FindSpawnLocation(IAtlas atlas, IRandom random, Box3 spawnBox)
        {
            return random.FindRandomInBox(spawnBox, test =>
            {
                var tile = atlas.GetTileAtPos(test);
                return tile.HasRoomForAgent;
            }).Value;
        }

    }
}
