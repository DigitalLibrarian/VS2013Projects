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

namespace Tiles.ScreensImpl.ContentFactories
{
    public class GameFactory
    {
        public Game SetupGenericZombieWorld(int seed = 42)
        {
            Vector2 siteSize = new Vector2(64, 64);
            var random = new RandomWrapper(new System.Random(seed));
            var siteFactory = new ZombieSiteFactory(random);
            return Setup(siteFactory, siteSize, random);
        }

        public Game SetupArenaWorld(int seed = 42)
        {
            Vector2 siteSize = new Vector2(64, 64);
            var random = new RandomWrapper(new System.Random(seed));
            var siteFactory = new ArenaSiteFactory(random);
            return Setup(siteFactory, siteSize, random);
        }

        private Game Setup(ISiteFactory siteFactory, Vector2 siteSize, IRandom random)
        {
            var atlas = new Atlas(siteFactory, siteSize);

            var actionLog = new ActionLog(maxLines: 10);
            var spawnBox = new Box(Vector2.Zero - siteSize, siteSize);
            var spawnPos = FindSpawnLocation(atlas, random, spawnBox);
            var player = CreatePlayer(random, atlas, spawnPos);
            var camera = new Camera(spawnPos);

            return new Game(atlas, player, camera, actionLog, random);
        }

        private IPlayer CreatePlayer(IRandom random, IAtlas atlas, Vector2 spawnPos)
        {
            return new AgentFactory(random).CreatePlayer(atlas, spawnPos);
        }

        Vector2 FindSpawnLocation(IAtlas atlas, IRandom random, Box spawnBox)
        {
            bool satisified = false;
            Vector2 test = Vector2.Zero;
            while(!satisified)
            {
                test = random.NextInBox(spawnBox);
                var tile = atlas.GetTileAtPos(test);

                if (tile.IsTerrainPassable)
                {
                    if (tile.HasStructureCell)
                    {
                        if (tile.StructureCell.CanPass)
                        {
                            satisified = true;
                        }
                    }
                    else
                    {
                        satisified = true;
                    }
                }
            }
            return test;
        }
    }
}
