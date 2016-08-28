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
using Tiles.Content.Map;
using Tiles.Agents.Combat;

namespace Tiles.ScreensImpl.SiteFactories
{
    public class DfTestSiteFactory : ISiteFactory
    {
        DfTagsFascade Df { get; set; }
        IRandom Random { get; set; }
        public DfTestSiteFactory(IDfObjectStore store, IRandom random)
        {
            Random = random;
            Df = new DfTagsFascade(store, random);
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

            if (!box.Contains(new Vector3(box.Min.X, box.Min.Y, 0)))
            {
                return s;
            }

            int numItems = box.Size.X;
            var metals = Df.GetMetalNames().ToList();
            var weapons = Df.GetWeaponNames().ToList();

            for (int i = 0; i < numItems; i++)
            {
                var spawnLoc = FindSpawnSitePos(s);
                if (spawnLoc.HasValue)
                {
                    var tile = s.GetTileAtSitePos(spawnLoc.Value);

                    var m = Random.NextElement(metals);
                    var w = Random.NextElement(weapons);
                    
                    tile.Items.Add(Df.CreateWeapon(w, m));
                }
            }

            int numAgents = numItems/2;
            var creatures = Df.GetCreatureNames().ToList();
            for (int i = 0; i < numAgents; i++)
            {
                var spawnLoc = FindSpawnSitePos(s);
                if (spawnLoc.HasValue)
                {
                    var tile = s.GetTileAtSitePos(spawnLoc.Value);
                    var creatureName = Random.NextElement(creatures);
                    var castes = Df.GetCreatureCastes(creatureName).ToList();

                    string caste = null;
                    if(castes.Any())
                    {
                        caste = Random.NextElement(castes);
                    }
                    var worldPos = s.Box.Min + spawnLoc.Value;

                    var agent = Df.CreateCreatureAgent(atlas, creatureName, caste, worldPos);
                    tile.SetAgent(agent);
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

            return Random.FindRandomInBox(box, test =>
            {
                var tile = s.GetTileAtSitePos(test);
                return tile.HasRoomForAgent;
            });
        }
    }
}