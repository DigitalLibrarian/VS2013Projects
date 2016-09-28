using DfNet.Raws;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Content.Bridge.DfNet;
using Tiles.Content.Map;
using Tiles.Ecs;
using Tiles.Injuries;
using Tiles.Items;
using Tiles.Materials;
using Tiles.Math;
using Tiles.Random;
using Tiles.ScreensImpl.SiteFactories;

namespace Tiles.EngineIntegrationTests
{
    public class DfContentTestBase
    {
        IRandom Random { get; set; }

        IDfObjectStore Store { get; set; }
        IDfAgentFactory DfAgentFactory { get; set; }
        IContentMapper ContentMapper { get; set; }

        IEntityManager EntityManager { get; set; }
        DfTagsFascade DfTagsFascade { get; set; }

        IAtlas Atlas { get; set; }

        protected ICombatMoveBuilder CombatMoveBuilder { get; set; }
        protected IInjuryReportCalc InjuryReportCalc { get; set; }

        public virtual void Initialize()
        {
            Store = TestContentStore.Get();
            DfAgentFactory = new DfAgentFactory(Store,
                new DfAgentBuilderFactory(),
                new DfColorFactory(),
                new DfMaterialFactory(Store, new DfMaterialBuilderFactory()),
                new DfTissueTemplateFactory(Store),
                new DfCombatMoveFactory(),
                new DfBodyAttackFactory()
                );

            ContentMapper = new ContentMapper();

            EntityManager = new EntityManager();
            DfTagsFascade = new DfTagsFascade(Store, EntityManager, Random);

            Atlas = new Mock<IAtlas>().Object;
            CombatMoveBuilder = new CombatMoveBuilder();
            InjuryReportCalc = new InjuryReportCalc(new LayeredMaterialStrikeResultBuilder(new MaterialStrikeResultBuilder()));
        }


        protected IAgent CreateAgent(string name, string caste, Vector3 pos)
        {
            return DfTagsFascade.CreateCreatureAgent(Atlas, name, caste, pos);
        }

        protected IItem CreateInorganicWeapon(string name, string materialName)
        {
            return DfTagsFascade.CreateInorganicWeapon(name, materialName);
        }

        protected IItem CreateMaterialTemplateWeapon(string name, string matTempName)
        {
            return DfTagsFascade.CreateMaterialTemplateWeapon(name, matTempName);
        }

        protected void AssertTissueInjuryClass(ITissueLayerInjuryClass expected, ITissueLayerInjury tInjury)
        {
            Assert.AreSame(expected, tInjury.Class, string.Format("Expected injury class {0} for tissue layer {1}, but got {2}", expected.Adjective, tInjury.Layer.Material.Name, tInjury.Class.Adjective));
        }
    }
}
