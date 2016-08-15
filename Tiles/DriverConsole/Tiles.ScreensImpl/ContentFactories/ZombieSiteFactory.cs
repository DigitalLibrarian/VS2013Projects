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
        GearFactory ItemFactory { get; set; }

        public ZombieSiteFactory(IRandom random)
        {
            Random = random;
            ItemFactory = new GearFactory(Random);
        }

        public ISite Create(IAtlas atlas, Vector3 siteIndex, Box3 box)
        {
            var s = new Site(box);

            foreach (var t in s.GetTiles())
            {
                SetupTile(Random, t);
            }

            int qW = box.Size.X / 8;
            int qH = box.Size.Y / 8;
            int qD = box.Size.Z / 8;
            var q = new Vector3(qW, qH, qD);
            var topLeft = Random.NextInBox(new Box3(Vector3.Zero, box.Size - q));
            //var buildingBox = new Box2(topLeft, topLeft + q);
            //var door = Random.NextElement(new List<CompassDirection> { CompassDirection.North, CompassDirection.East, CompassDirection.South, CompassDirection.West });
            //CreateRectangularBuilding(s, buildingBox, door);
            
            for (int i = 0; i < box.Size.X; i++)
            {
                var spawnLoc = FindSpawnSitePos(s);
                if (spawnLoc.HasValue)
                {
                    var tile = s.GetTileAtSitePos(spawnLoc.Value);
                    tile.Items.Add(ItemFactory.CreateRandomItem());
                }
            }

            var agentFactory = new AgentFactory(Random);
            var numZombies = 10;
            for (int i = 0; i < numZombies; i++)
                AddZombie(atlas, s, agentFactory);

            var numSurvivors = 1;
            for (int i = 0; i < numSurvivors; i++)
                AddSurvivor(atlas, s, agentFactory);

            return s;
        }

        Vector3? FindSpawnSitePos(ISite s)
        {
            var targetSiteZ = -s.Box.Min.Z;
            var box = new Box3(
                Vector3.Zero,
                new Vector3(s.Box.Size.X, s.Box.Size.Y, 1)
                );
            Vector3? test = null;
            bool satisfied = false;
            while(!satisfied)
            {
                test = Random.NextInBox(box);
                var tile = s.GetTileAtSitePos(test.Value);

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

            if (!satisfied)
            {
                test = null;
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

        private void CreateRectangularBuilding(ISite site, Box3 box, CompassDirection door)
        {
            var fact = new StructureFactory();
            var insertionPoint = box.Min;
            var size = box.Size;

            //var structure = fact.CreateRectangularBuilding(size, door);
            //site.InsertStructure(insertionPoint, structure);
        }

        
        void AddZombie(IAtlas atlas, ISite site, AgentFactory agentFactory)
        {
            var sitePos = FindSpawnSitePos(site);
            if (sitePos.HasValue)
            {
                var worldPos = site.Box.Min + sitePos.Value;
                var zombie = agentFactory.CreateZombieAgent(atlas, worldPos);
                var spawnTile = site.GetTileAtSitePos(sitePos.Value);
                spawnTile.SetAgent(zombie);
            }
        }

        void AddSurvivor(IAtlas atlas, ISite site, AgentFactory agentFactory)
        {
            var sitePos = FindSpawnSitePos(site);
            if (sitePos.HasValue)
            {
                var worldPos = site.Box.Min + sitePos.Value;
                var survivor = agentFactory.CreateSurvivor(atlas, worldPos);
                var spawnTile = site.GetTileAtSitePos(sitePos.Value);
                spawnTile.SetAgent(survivor);
            }
        }
    }
}
