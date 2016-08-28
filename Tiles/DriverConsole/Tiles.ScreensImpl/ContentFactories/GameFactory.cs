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



namespace Tiles.ScreensImpl.ContentFactories
{
    public class GameFactory
    {
        public IGame SetupGenericZombieWorld(int seed = 42)
        {
            Vector3 siteSize = new Vector3(64, 64, 64);
            var random = new RandomWrapper(new System.Random(seed));
            var siteFactory = new ZombieSiteFactory(random);
            return Setup(siteFactory, siteSize, random);
        }

        public IGame SetupArenaWorld(string dfRawDir, int seed = 42)
        {
            var siteSize = new Vector3(64, 64, 64);
            var random = new RandomWrapper(new System.Random(seed));

            var dfStore = DfObjectStore.CreateFromDirectory(dfRawDir);
            var siteFactory = new ArenaSiteFactory(dfStore, random);
            return Setup(siteFactory, siteSize, random);
        }

        public IGame SetupDfTestWorld(string dfRawDir, int seed = 42)
        {
            var dfStore = DfObjectStore.CreateFromDirectory(dfRawDir);

            var siteSize = new Vector3(64, 64, 64);
            var random = new RandomWrapper(new System.Random(seed));
            var siteFactory = new DfTestSiteFactory(dfStore, random);
            return Setup(siteFactory, siteSize, random);

        }

        private Game Setup(ISiteFactory siteFactory, Vector3 siteSize, IRandom random)
        {
            var atlas = new Atlas(siteFactory, siteSize);

            var actionLog = new ActionLog(maxLines: 10);
            
            var spawnBox = new Box3(
                Vector3.Zero,
                new Vector3(siteSize.X, siteSize.Y, 1)
                );
            
            var spawnPos = FindSpawnLocation(atlas, random, spawnBox);
            var player = CreatePlayer(random, atlas, spawnPos);
            var camera = new Camera(spawnPos);

            return new Game(atlas, player, camera, actionLog, random);
        }

        private IPlayer CreatePlayer(IRandom random, IAtlas atlas, Vector3 spawnPos)
        {
            return new HardCodedAgentFactory(random).CreatePlayer(atlas, spawnPos);
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
