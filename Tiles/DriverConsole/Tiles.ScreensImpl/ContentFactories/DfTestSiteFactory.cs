﻿using System;
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
using Tiles.Content.Bridge.DfNet;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class DfTestSiteFactory : ISiteFactory
    {
        IDfObjectStore Store { get; set; }
        IDfMaterialFactory MaterialFactory { get; set; }
        IDfItemFactory ItemFactory { get; set; }
        IRandom Random { get; set; }
        public DfTestSiteFactory(IDfObjectStore store, IRandom random)
        {
            Store = store;
            Random = random;
            ItemFactory = new DfItemFactory(Store, new DfItemBuilderFactory());
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

            var metals = Store.Get(DfTags.MATERIAL_TEMPLATE)
                            .Where(o => o.Tags.Any(t => t.IsSingleWord(DfTags.MiscTags.IS_METAL)))
                            .SelectMany(matTemp =>
                            {
                                return Store.Get(DfTags.INORGANIC)
                                        .Where(inOrg => inOrg.Tags.Any(
                                                    t => t.Name.Equals(DfTags.MiscTags.USE_MATERIAL_TEMPLATE)
                                                        && t.GetParam(0).Equals(matTemp.Name)));
                            }).Select(t => t.Name).ToList();

            var weapons = Store.Get(DfTags.ITEM_WEAPON).Select(t => t.Name).ToList();

            for (int i = 0; i < numItems; i++)
            {
                var spawnLoc = FindSpawnSitePos(s);
                if (spawnLoc.HasValue)
                {
                    var tile = s.GetTileAtSitePos(spawnLoc.Value);

                    var m = Random.NextElement(metals);
                    var w = Random.NextElement(weapons);




                    // Now to map to engine types from the model types

                    //tile.Items.Add(Fab.CreateWeapon(m, w));
                }
            }

            /*
            var metals = Fab.Inorganics.Where(x =>
            {
                if (x.UseMaterialTemplate == null) return false;
                var mt = Fab.MaterialTemplates.Single(temp => temp.ReferenceName.Equals(x.UseMaterialTemplate));

                return mt.IsMetal;
            }).ToList();
            var weapons = Fab.ItemWeapons.ToList();

            for (int i = 0; i < numItems; i++)
            {
                var spawnLoc = FindSpawnSitePos(s);
                if (spawnLoc.HasValue)
                {
                    var tile = s.GetTileAtSitePos(spawnLoc.Value);
                    
                    var m = Random.NextElement(metals);
                    var w = Random.NextElement(weapons);

                    tile.Items.Add(Fab.CreateWeapon(m, w));
                }
            }
             * */
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
