using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Behaviors;
using Tiles.Agents;
using Tiles.Items;
using Tiles.Math;
using Tiles.Random;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class ZombieSiteFactory : ISiteFactory
    {
        IRandom Random { get; set; }

        public ZombieSiteFactory(IRandom random)
        {
            Random = random;
        }

        public ISite Create(IAtlas atlas, Vector2 siteIndex, Box box)
        {
            var s = new Site(box);

            foreach (var t in s.GetTiles())
            {
                SetupTile(Random, t);
            }

            int qW = box.Size.X / 8;
            int qH = box.Size.Y / 8;
            var q = new Vector2(qW, qH);
            var topLeft = Random.NextInBox(new Box(Vector2.Zero, box.Size - q));
            var buildingBox = new Box(topLeft, topLeft + q);
            var door = Random.NextElement(new List<CompassDirection> { CompassDirection.North, CompassDirection.East, CompassDirection.South, CompassDirection.West });
            CreateRectangularBuilding(s, buildingBox, door);
            
            var itemFactory = new ItemFactory();
            for (int i = 0; i < box.Size.X; i++)
            {
                var spawnLoc = FindSpawnSitePos(s);
                s.GetTileAtSitePos(spawnLoc).Items.Add(itemFactory.CreateRandomItem(Random));
            }

            var agentFactory = new AgentFactory(Random);
            var numZombies = 2;
            for (int i = 0; i < numZombies; i++)
                AddZombie(atlas, s, agentFactory);

            var numSurvivors = 2;
            for (int i = 0; i < numSurvivors; i++)
                AddSurvivor(atlas, s, agentFactory);

            return s;
        }

        Vector2 FindSpawnSitePos(ISite s)
        {
            Vector2 test = Vector2.Zero;
            bool satisfied = false;
            while(!satisfied)
            {
                test = Random.Next(s.Box.Size);
                var tile = s.GetTileAtSitePos(test);

                if (!tile.HasAgent)
                {
                    if (tile.IsTerrainPassable)
                    {
                        if (tile.HasStructureCell)
                        {
                            if (tile.StructureCell.CanPass)
                            {
                                satisfied = true;
                            }
                        }
                        else
                        {
                            satisfied = true;
                        }
                    }
                }
            }
            return test;
        }

        static Dictionary<Terrain, Symbol> TerrainSymbolMap = new Dictionary<Terrain, Symbol>
        {
            {Terrain.None, Symbol.Terrain_Empty },
            {Terrain.Rock, Symbol.Terrain_Floor },
            {Terrain.Water, Symbol.Liquid_Medium},
            {Terrain.Lava, Symbol.Liquid_Dark},
            {Terrain.Mud, Symbol.Liquid_Light },
            {Terrain.Tree, Symbol.Terrain_Tree}
        };

        private static void SetupTile(IRandom random, ITile tile)
        {
            var d = new List<dynamic>{
                new { Terrain = Terrain.Lava, Frequency = 0.001d, IsPassable = false, FG = Color.DarkRed, BG = Color.Red, Symbol = Symbol.Liquid_Dark},
                new { Terrain = Terrain.Mud, Frequency = 0.001d, IsPassable = true, FG = Color.DarkGray, BG = Color.Black, Symbol = Symbol.Liquid_Light}, 
                new { Terrain = Terrain.Rock, Frequency = 0.005d, IsPassable = false, FG = Color.White, BG = Color.Black, Symbol = Symbol.Terrain_Floor}, 
                new { Terrain = Terrain.Tree, Frequency = 0.1d, IsPassable = false, FG = Color.Green, BG = Color.Black, Symbol = Symbol.Terrain_Tree}, 
                new { Terrain = Terrain.None, Frequency = 1d, IsPassable = true, FG = Color.Black, BG = Color.Black, Symbol = Symbol.None}, 
            };

            bool finished = false;
            while (!finished)
            {
                foreach (var r in d.OrderBy(x => x.Frequency))
                {
                    var rand = random.NextDouble();
                    if (rand < r.Frequency)
                    {
                        tile.Terrain = r.Terrain;
                        tile.IsTerrainPassable = r.IsPassable;

                        tile.TerrainSprite = new Sprite(r.Symbol, r.FG, r.BG);
                        finished = true;
                        break;
                    }
                }
            }
        }

        private void CreateRectangularBuilding(ISite site, Box box, CompassDirection door)
        {
            var fact = new StructureFactory();
            var insertionPoint = box.Min;
            var size = box.Size;

            var structure = fact.CreateRectangularBuilding(size, door);
            site.InsertStructure(insertionPoint, structure);
        }

        
        void AddZombie(IAtlas atlas, ISite site, AgentFactory agentFactory)
        {
            var sitePos = FindSpawnSitePos(site);
            var worldPos = site.Box.Min + sitePos;
            var zombie = agentFactory.CreateZombieAgent(atlas, worldPos);
            var spawnTile = site.GetTileAtSitePos(sitePos);
            spawnTile.SetAgent(zombie);
        }

        void AddSurvivor(IAtlas atlas, ISite site, AgentFactory agentFactory)
        {
            var sitePos = FindSpawnSitePos(site);
            var worldPos = site.Box.Min + sitePos;
            var survivor = agentFactory.CreateSurvivor(atlas, worldPos);
            var spawnTile = site.GetTileAtSitePos(sitePos);
            spawnTile.SetAgent(survivor);
        }
    }
}
