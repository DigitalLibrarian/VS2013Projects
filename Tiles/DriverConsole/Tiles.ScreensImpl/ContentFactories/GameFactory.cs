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
        IRandom Random {  get; set;}
        AgentFactory AgentFactory { get; set; }
        ZombieSiteFactory SiteFactory { get; set; }

        IActiveAgentCommandSet ActiveCommandSet { get; set; }

        public GameFactory(int seed = 42)
        {
            Random = new RandomWrapper(new System.Random(seed));
            var tracker = new AgentCommandContextTracker();
            AgentFactory = new AgentFactory(Random, tracker);
            SiteFactory = new ZombieSiteFactory(Random, AgentFactory);
            ActiveCommandSet = tracker;
        }


        public Game SetupGenericZombieWorld(int seed = 42)
        {
            Vector2 siteSize = new Vector2(64, 64);

            var atlas = new Atlas(SiteFactory, siteSize);

            var actionLog = new ActionLog(maxLines: 10);
            var spawnBox = new Box(Vector2.Zero - siteSize, siteSize);
            var spawnPos = FindSpawnLocation(atlas, spawnBox);
            var player = CreatePlayer(atlas, spawnPos);
            var camera = new Camera(spawnPos);

            return new Game(atlas, player, camera, actionLog, ActiveCommandSet, Random);
        }

        private IPlayer CreatePlayer(IAtlas atlas, Vector2 spawnPos)
        {
            return AgentFactory.CreatePlayer(atlas, spawnPos);
        }

        Vector2 FindSpawnLocation(IAtlas atlas, Box spawnBox)
        {
            bool satisified = false;
            Vector2 test = Vector2.Zero;
            while(!satisified)
            {
                test = Random.NextInBox(spawnBox);
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
