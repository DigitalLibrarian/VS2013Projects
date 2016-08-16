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
using DwarfFortressNet.Bridge;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class DfTestSiteFactory : ISiteFactory
    {
        DfFabricator Fab { get; set; }
        IRandom Random { get; set; }
        public DfTestSiteFactory(DfFabricator fab, IRandom random)
        {
            Fab = fab;
            Random = random;
        }

        public ISite Create(IAtlas atlas, Vector3 siteIndex, Box3 box)
        {
            var s = new Site(box);

            foreach (var tile in s.GetTiles())
            {
                tile.TerrainSprite = new Sprite(Symbol.None, Color.White, Color.Black);
                tile.IsTerrainPassable = true;
                tile.Terrain = Terrain.None;
            }

            int numItems = box.Size.X;

            for (int i = 0; i < numItems; i++)
            {
                var spawnLoc = FindSpawnSitePos(s);
                if (spawnLoc.HasValue)
                {
                    var tile = s.GetTileAtSitePos(spawnLoc.Value);
                    var m = Random.NextElement(Fab.Inorganics.ToList());
                    var w = Random.NextElement(Fab.ItemWeapons.ToList());

                    tile.Items.Add(Fab.CreateWeapon(m, w));
                }
            }
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
            while (!satisfied)
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
    }
}
