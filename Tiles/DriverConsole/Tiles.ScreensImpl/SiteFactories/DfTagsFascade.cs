using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Behaviors;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Content.Bridge.DfNet;
using Tiles.Content.Map;
using Tiles.Items;
using Tiles.Materials;
using Tiles.Math;
using Tiles.Random;

namespace Tiles.ScreensImpl.SiteFactories
{
    public class DfTagsFascade
    {
        IDfObjectStore Store { get; set; }
        IDfMaterialFactory DfMaterialFactory { get; set; }
        IDfItemFactory DfItemFactory { get; set; }
        IDfAgentFactory DfAgentFactory { get; set; }
        IAgentFactory AgentFactory { get; set; }
        IItemFactory ItemFactory { get; set; }
        IRandom Random { get; set; }
        IContentMapper ContentMapper { get; set; }
        IAgentCommandPlanner DefaultPlanner { get; set; }

        public DfTagsFascade(IDfObjectStore store, IRandom random)
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

            DefaultPlanner = new DefaultAgentCommandPlanner(random,
                new AgentCommandFactory(),
                new CombatMoveDiscoverer(new CombatMoveBuilder(new DamageCalc())),
                new PositionFinder());
        }

        public IAgent CreateCreatureAgent(IAtlas atlas, string name, string caste, Vector3 pos)
        {
            var agentContent = DfAgentFactory.Create(name, caste);
            var engineAgentClass = ContentMapper.Map(agentContent);
            return AgentFactory.Create(atlas, engineAgentClass,pos, DefaultPlanner);
        }

        public IEnumerable<string> GetCreatureNames()
        {
            return Store.Get(DfTags.CREATURE)
                .Where(IsValidCreature)
                .Select(x => x.Name).ToList();
        }

        bool IsValidCreature(DfObject o)
        {
            return o.Tags.Any(t => t.Name.Equals("BODY"));
        }

        public IEnumerable<string> GetCreatureCastes(string creatureName)
        {
            var creatureDf = Store.Get(DfTags.CREATURE, creatureName);
            return creatureDf.Tags
                        .Where(t => t.Name.Equals(DfTags.MiscTags.CASTE))
                        .Select(t => t.GetParam(0))
                        .ToList();
        }

        public IEnumerable<string> GetMetalNames()
        {
            return Store.Get(DfTags.MATERIAL_TEMPLATE)
                            .Where(o => o.Tags.Any(t => t.IsSingleWord(DfTags.MiscTags.IS_METAL)))
                            .SelectMany(matTemp =>
                            {
                                return Store.Get(DfTags.INORGANIC)
                                        .Where(inOrg => inOrg.Tags.Any(
                                                    t => t.Name.Equals(DfTags.MiscTags.USE_MATERIAL_TEMPLATE)
                                                        && t.GetParam(0).Equals(matTemp.Name)));
                            }).Select(t => t.Name).ToList();
        }

        public IEnumerable<string> GetWeaponNames()
        {
            return Store.Get(DfTags.ITEM_WEAPON).Select(t => t.Name).ToList();
        }

        
        public IItem CreateWeapon(string w, string m)
        {
            var materialContent = DfMaterialFactory.CreateInorganic(m); 
            var content = DfItemFactory.Create(DfTags.ITEM_WEAPON, w, materialContent);
            return ItemFactory.Create(ContentMapper.Map(content));
        }
    }
}
