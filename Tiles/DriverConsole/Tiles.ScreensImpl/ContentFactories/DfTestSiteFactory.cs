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
using Tiles.Content.Bridge.DfNet;
using Tiles.Content.Map;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class DfTestSiteFactory : ISiteFactory
    {
        IDfObjectStore Store { get; set; }
        IDfMaterialFactory DfMaterialFactory { get; set; }
        IDfItemFactory DfItemFactory { get; set; }
        IDfAgentFactory DfAgentFactory { get; set; }
        IAgentFactory AgentFactory { get; set;}
        IItemFactory ItemFactory { get; set; }
        IRandom Random { get; set; }
        IContentMapper ContentMapper { get; set; }

        IAgentCommandPlanner DefaultPlanner { get; set; }

        public DfTestSiteFactory(IDfObjectStore store, IRandom random)
        {
            Store = store;
            Random = random;
            ItemFactory = new ItemFactory();
            AgentFactory = new AgentFactory(new BodyFactory(new TissueFactory()));
            DfMaterialFactory = new DfMaterialFactory(Store);
            var moveFactory = new DfCombatMoveFactory();
            DfItemFactory = new DfItemFactory(Store, new DfItemBuilderFactory(), moveFactory);
            DfAgentFactory = new DfAgentFactory(Store, new DfAgentBuilderFactory(), DfMaterialFactory, moveFactory);
            ContentMapper = new ContentMapper();

            DefaultPlanner = new DefaultAgentCommandPlanner(random, new AgentCommandFactory());
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

                    var materialContent = DfMaterialFactory.CreateInorganic(m);
                    var weaponItemContent = DfItemFactory.Create(DfTags.ITEM_WEAPON, w, materialContent);
                    var engineItemClass = ContentMapper.Map(weaponItemContent);
                    

                    tile.Items.Add(ItemFactory.Create(engineItemClass));
                }
            }

            int numAgents = box.Size.X / 2;
            var creatures = Store.Get(DfTags.CREATURE).ToList();
            for (int i = 0; i < numAgents; i++)
            {
                var spawnLoc = FindSpawnSitePos(s);
                if (spawnLoc.HasValue)
                {
                    var tile = s.GetTileAtSitePos(spawnLoc.Value);
                    var creatureDf = Random.NextElement(creatures);
                    var castes = creatureDf.Tags
                        .Where(t => t.Name.Equals(DfTags.MiscTags.CASTE))
                        .Select(t => t.GetParam(0))
                        .ToList();

                    string caste = null;
                    if(castes.Any())
                    {
                        caste = Random.NextElement(castes);
                    }
                    var agentContent = DfAgentFactory.Create(creatureDf.Name);
                    var engineAgentClass = ContentMapper.Map(agentContent);
                    var agent = AgentFactory.Create(atlas, engineAgentClass, spawnLoc.Value, DefaultPlanner);
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
